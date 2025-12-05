using System.Text.Json;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PoliticalAppAPI.Data;
using PoliticalAppAPI.Models;

namespace PoliticalAppAPI.Services
{
    public class BillSyncService : IBillSyncService
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly ILogger<BillSyncService> _logger;

        private static readonly JsonSerializerOptions JsonOpts =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public BillSyncService(
            AppDbContext db,
            IHttpClientFactory httpFactory,
            IConfiguration config,
            ILogger<BillSyncService> logger)
        {
            _db = db;
            _http = httpFactory.CreateClient("CongressGov");
            _apiKey = config["CongressGov:ApiKey"]
                ?? throw new InvalidOperationException("CongressGov:ApiKey is not configured");
            _logger = logger;
        }

        public async Task<(IReadOnlyList<Bill> Bills, int Total)> GetPagedAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            // Base query: newest first
            var baseQuery = _db.Bills
                .OrderByDescending(b => b.LatestActionDate ?? b.LastUpdatedUtc);

            var totalInDb = await baseQuery.CountAsync();
            var skip = (page - 1) * pageSize;

            // If we already have enough rows to satisfy this page, just return cached data
            if (totalInDb >= skip + pageSize)
            {
                var cached = await baseQuery
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                return (cached, totalInDb);
            }

            // Otherwise, fetch this page from Congress.gov
            List<Bill> fresh = new();
            try
            {
                fresh = await FetchBillsPageFromCongressAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bills page {Page} from Congress.gov", page);
            }

            if (fresh.Count > 0)
            {
                // Avoid duplicates: check if each bill already exists before inserting
                foreach (var bill in fresh)
                {
                    bool exists = await _db.Bills.AnyAsync(b =>
                        b.Congress == bill.Congress &&
                        b.BillType == bill.BillType &&
                        b.BillNumber == bill.BillNumber);

                    if (!exists)
                    {
                        _db.Bills.Add(bill);
                    }
                }

                try
                {
                    await _db.SaveChangesAsync();
                }
                // If the UNIQUE KEY still catches a race-condition duplicate, ignore it
                catch (DbUpdateException ex) when (ex.InnerException is MySqlConnector.MySqlException sqlEx &&
                                                sqlEx.Number == 1062) // duplicate key
                {
                    _logger.LogWarning(ex, "Duplicate bill encountered while saving; ignoring.");
                }

                // Refresh totals after insert
                baseQuery = _db.Bills
                    .OrderByDescending(b => b.LatestActionDate ?? b.LastUpdatedUtc);
                totalInDb = await baseQuery.CountAsync();
            }

            var items = await baseQuery
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            await EnsureSummariesAsync(items);
            return (items, totalInDb);
        }

        public async Task<string?> GetOrFetchSummaryAsync(int billId)
        {
            var bill = await _db.Bills.FindAsync(billId);
            if (bill is null) return null;

            if (!string.IsNullOrWhiteSpace(bill.SummaryText))
            {
                return bill.SummaryText;
            }

            try
            {
                var summary = await FetchSummaryFromCongressAsync(
                    bill.Congress, bill.BillType, bill.BillNumber);

                bill.SummaryText = summary;
                bill.LastUpdatedUtc = DateTime.UtcNow;
                await _db.SaveChangesAsync();

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching summary for bill {Id}", billId);
                return null;
            }
        }

        // -------- Congress.gov calls --------
        private async Task<List<Bill>> FetchBillsPageFromCongressAsync(int page, int pageSize)
        {
            var list = new List<Bill>();
            var now = DateTime.UtcNow;

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            int offset = (page - 1) * pageSize;
            int limit = pageSize;

            string url =
                $"/v3/bill" +
                $"?api_key={_apiKey}&format=json" +
                $"&limit={limit}&offset={offset}";

            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Congress.gov error {StatusCode} {Reason} for URL {Url}",
                    response.StatusCode, response.ReasonPhrase, url);

                return list;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("bills", out var billsEl) ||
                billsEl.ValueKind != JsonValueKind.Array)
            {
                return list;
            }

            foreach (var billEl in billsEl.EnumerateArray())
            {
                var parsed = await ParseBill(billEl, now);
                if (parsed != null)
                    list.Add(parsed);
            }

            return list;
        }



        private async Task<Bill?> ParseBill(JsonElement b, DateTime now)
        {
            try
            {
                // congress may not exist in some endpoints
                int congress = b.TryGetProperty("congress", out var cEl) &&
                            cEl.ValueKind == JsonValueKind.Number
                                ? cEl.GetInt32()
                                : 0;

                // bill type (HR, S, HJRES, etc.)
                string billType = b.TryGetProperty("type", out var typeEl) &&
                                typeEl.ValueKind == JsonValueKind.String
                                    ? typeEl.GetString() ?? ""
                                    : "";

                // bill number (string → int)
                int billNumber = 0;
                if (b.TryGetProperty("number", out var numEl) &&
                    numEl.ValueKind == JsonValueKind.String)
                {
                    int.TryParse(numEl.GetString(), out billNumber);
                }

                if (string.IsNullOrWhiteSpace(billType) || billNumber == 0)
                    return null;

                // title
                string title = $"{billType} {billNumber}";
                if (b.TryGetProperty("title", out var tEl) &&
                    tEl.ValueKind == JsonValueKind.String)
                {
                    title = tEl.GetString() ?? title;
                }

                // latest action
                DateTime? latestActionDate = null;
                string? latestActionText = null;

                if (b.TryGetProperty("latestAction", out var laEl) &&
                    laEl.ValueKind == JsonValueKind.Object)
                {
                    if (laEl.TryGetProperty("actionDate", out var adEl) &&
                        DateTime.TryParse(adEl.GetString(), out var d))
                    {
                        latestActionDate = d;
                    }

                    if (laEl.TryGetProperty("text", out var textEl))
                        latestActionText = textEl.GetString();
                }

                // policy area
                string? policyArea = null;
                if (b.TryGetProperty("policyArea", out var paEl))
                {
                    if (paEl.ValueKind == JsonValueKind.Object &&
                        paEl.TryGetProperty("name", out var nameEl))
                    {
                        policyArea = nameEl.GetString();
                    }
                    else if (paEl.ValueKind == JsonValueKind.String)
                    {
                        policyArea = paEl.GetString();
                    }
                }

                // sponsor
                string? sponsorName = null;

                if (b.TryGetProperty("sponsors", out var sponsorsEl) &&
                    sponsorsEl.ValueKind == JsonValueKind.Array)
                {
                    var first = sponsorsEl.EnumerateArray().FirstOrDefault();
                    if (first.ValueKind == JsonValueKind.Object &&
                        first.TryGetProperty("fullName", out var fullNameEl))
                    {
                        sponsorName = fullNameEl.GetString();
                    }
                }
                else if (b.TryGetProperty("sponsor", out var sponsorEl) &&
                        sponsorEl.ValueKind == JsonValueKind.Object &&
                        sponsorEl.TryGetProperty("fullName", out var snEl))
                {
                    sponsorName = snEl.GetString();
                }
                
                string? summary = null;

                if (congress > 0)
                {
                    summary = await FetchSummaryFromCongressAsync(congress, billType, billNumber);
                    Console.WriteLine("Fetched summary for bill: " +
                        (summary?.Substring(0, Math.Min(50, summary.Length)) ?? "null"));
                }

                return new Bill
                {
                    Congress = congress,
                    BillType = billType,
                    BillNumber = billNumber,
                    Title = title,
                    LatestActionDate = latestActionDate,
                    LatestActionText = latestActionText,
                    PolicyArea = policyArea,
                    SponsorName = sponsorName,
                    SummaryText = summary,
                    LastUpdatedUtc = now
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse bill JSON: {Json}", b.GetRawText());
                return null;
            }
        }

        private async Task<string?> FetchSummaryFromCongressAsync(int congress, string billType, int billNumber)
        {
            var url =
                $"/v3/bill/{congress}/{billType.ToLowerInvariant()}/{billNumber}/summaries" +
                $"?api_key={_apiKey}&format=json";

            Console.WriteLine("Fetching summary from URL: " + url);

            var json = await _http.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            // The data array is usually called "summaries" for this endpoint.
            if (!root.TryGetProperty("summaries", out var summariesEl) ||
                summariesEl.ValueKind != JsonValueKind.Array ||
                summariesEl.GetArrayLength() == 0)
            {
                return null;
            }

            var first = summariesEl[0];

            if (!first.TryGetProperty("text", out var textEl))
                return null;

            var full = textEl.GetString();
            if (string.IsNullOrWhiteSpace(full))
                return null;

            // You can trim here if you ever want a shorter snippet
            return NormalizeSummaryToPlainText(full);
        }

        private async Task EnsureSummariesAsync(IEnumerable<Bill> bills, CancellationToken ct = default)
        {
            var needs = bills
                .Where(b => b.Congress > 0 && string.IsNullOrWhiteSpace(b.SummaryText))
                .ToList();

            if (needs.Count == 0)
                return;

            foreach (var bill in needs)
            {
                try
                {
                    var summary = await FetchSummaryFromCongressAsync(
                        bill.Congress, bill.BillType, bill.BillNumber);

                    if (!string.IsNullOrWhiteSpace(summary))
                    {
                        bill.SummaryText = summary;
                        bill.LastUpdatedUtc = DateTime.UtcNow;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Failed to fetch summary for {Type} {Number} ({Congress})",
                        bill.BillType, bill.BillNumber, bill.Congress);
                }
            }

            await _db.SaveChangesAsync(ct);
        }

        private static string? NormalizeSummaryToPlainText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return null;

            // Turn paragraph / <br> tags into line breaks first
            html = Regex.Replace(html, @"</p\s*>", "\n\n", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);

            // Strip all remaining tags
            var text = Regex.Replace(html, "<.*?>", string.Empty);

            // Decode HTML entities (&nbsp;, &amp;, etc.)
            text = WebUtility.HtmlDecode(text);

            // Normalize whitespace a bit
            text = Regex.Replace(text, @"[ \t]{2,}", " ");
            text = Regex.Replace(text, @"\n{3,}", "\n\n");

            return text.Trim();
        }
    }
}
