using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PoliticalAppAPI.Data;
using PoliticalAppAPI.Models;
using PoliticalAppAPI.Helpers;

namespace PoliticalAppAPI.Services
{
    public class RepresentativeSyncService : IRepresentativeSyncService
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly ILogger<RepresentativeSyncService> _logger;

        private static readonly JsonSerializerOptions JsonOpts =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public RepresentativeSyncService(
            AppDbContext db,
            IHttpClientFactory httpFactory,
            IConfiguration config,
            ILogger<RepresentativeSyncService> logger)
        {
            _db = db;
            _http = httpFactory.CreateClient("CongressGov");
            _logger = logger;

            _apiKey = config["CongressGov:ApiKey"]
                ?? throw new InvalidOperationException("CongressGov:ApiKey is not configured");
        }

        // ---------- Public API used by controller ----------

        public async Task<List<Representative>> GetOrRefreshByStateAsync(string stateCode)
        {
            var code = stateCode.ToUpperInvariant();

            // 1) Try cached data
            var existing = await _db.Representatives
                .Where(r => r.StateCode == code)
                .ToListAsync();

            if (existing.Any())
                return existing;

            // 2) Try to fetch from Congress.gov, but NEVER crash if it fails
            List<Representative> fresh = new();

            try
            {
                fresh = await FetchFromCongressByStateAsync(code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching representatives for state {StateCode} from Congress.gov", code);
                // fall back to existing (likely empty)
                return existing;
            }

            if (!fresh.Any())
            {
                _logger.LogWarning("No representatives returned from Congress.gov for state {StateCode}", code);
                return existing;
            }

            // 3) Save to DB
            await _db.Representatives.AddRangeAsync(fresh);
            await _db.SaveChangesAsync();

            return fresh;
        }

        public async Task<List<Representative>> GetOrRefreshAllAsync()
        {
            // 1) Try cached data
            var existing = await _db.Representatives.ToListAsync();
            if (existing.Any())
                return existing;

            // 2) Try Congress.gov, but NEVER throw all the way up
            List<Representative> fresh = new();

            try
            {
                fresh = await FetchAllFromCongressAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ALL representatives from Congress.gov");
                return existing;
            }

            if (!fresh.Any())
            {
                _logger.LogWarning("No representatives returned from Congress.gov for ALL members");
                return existing;
            }

            // 3) Save to DB
            await _db.Representatives.AddRangeAsync(fresh);
            await _db.SaveChangesAsync();

            return fresh;
        }

        // ---------- Congress.gov calls ----------

        private async Task<List<Representative>> FetchFromCongressByStateAsync(string stateCode)
        {
            var url = $"/v3/member/{stateCode}?currentMember=true&limit=250&format=json&api_key={_apiKey}";
            _logger.LogInformation("Calling Congress.gov (state): {Url}", url);

            var response = await _http.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                _logger.LogError("Congress.gov returned 403 Forbidden for state {StateCode}. Check API key / permissions.", stateCode);
                return new List<Representative>();
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Congress.gov returned {StatusCode} for state {StateCode}: {Reason}",
                    (int)response.StatusCode, stateCode, response.ReasonPhrase);
                return new List<Representative>();
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return ParseMembers(doc.RootElement);
        }

        private async Task<List<Representative>> FetchAllFromCongressAsync()
        {
            const int pageSize = 250;
            var all = new List<Representative>();
            var offset = 0;

            while (true)
            {
                var url = $"/v3/member?currentMember=true&limit={pageSize}&offset={offset}&format=json&api_key={_apiKey}";
                _logger.LogInformation("Calling Congress.gov (all) with offset {Offset}: {Url}", offset, url);

                var response = await _http.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    _logger.LogError("Congress.gov returned 403 Forbidden while paging members. Stopping.");
                    break;
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Congress.gov returned {StatusCode} while paging members: {Reason}",
                        (int)response.StatusCode, response.ReasonPhrase);
                    break;
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var batch = ParseMembers(doc.RootElement);

                if (batch.Count == 0)
                {
                    // no more records
                    break;
                }

                all.AddRange(batch);
                offset += pageSize;
            }

            return all;
        }


        private static List<Representative> ParseMembers(JsonElement root)
        {
            var list = new List<Representative>();

            if (!root.TryGetProperty("members", out var members) ||
                members.ValueKind != JsonValueKind.Array)
            {
                return list;
            }

            var now = DateTime.UtcNow;

            foreach (var m in members.EnumerateArray())
            {
                string bioguideId = m.GetProperty("bioguideId").GetString() ?? string.Empty;
                string name = m.GetProperty("name").GetString() ?? string.Empty;
                string partyName = m.GetProperty("partyName").GetString() ?? string.Empty;

                // Congress.gov "state" is often the full name ("Florida", "California")
                string rawState = m.GetProperty("state").GetString() ?? string.Empty;
                rawState = rawState.Trim();

                string stateName = rawState;
                string stateCode;

                // Try to convert full name -> 2-letter code, or treat as code if it's 2 chars
                if (StateMaps.NameToCode.TryGetValue(rawState, out var codeFromName))
                {
                    stateCode = codeFromName.ToUpperInvariant();
                }
                else if (rawState.Length == 2 && StateMaps.CodeToName.TryGetValue(rawState, out var nameFromCode))
                {
                    stateCode = rawState.ToUpperInvariant();
                    stateName = nameFromCode;
                }
                else
                {
                    // Fallback: store first 2 chars as code, keep full string as name
                    stateCode = rawState.Length >= 2
                        ? rawState.Substring(0, 2).ToUpperInvariant()
                        : rawState.ToUpperInvariant();
                }

                int? districtNumber = null;
                if (m.TryGetProperty("district", out var distEl) &&
                    distEl.ValueKind == JsonValueKind.Number &&
                    distEl.TryGetInt32(out var distNum))
                {
                    districtNumber = distNum;
                }

                string chamber = "";
                int startYear = 0;
                if (m.TryGetProperty("terms", out var terms) &&
                    terms.TryGetProperty("item", out var items) &&
                    items.ValueKind == JsonValueKind.Array &&
                    items.GetArrayLength() > 0)
                {
                    var lastTerm = items.EnumerateArray().Last();
                    if (lastTerm.TryGetProperty("chamber", out var chEl))
                        chamber = chEl.GetString() ?? "";
                    if (lastTerm.TryGetProperty("startYear", out var yEl) &&
                        yEl.ValueKind == JsonValueKind.Number &&
                        yEl.TryGetInt32(out var y))
                        startYear = y;
                }

                string? imageUrl = null;
                if (m.TryGetProperty("depiction", out var depEl) &&
                    depEl.TryGetProperty("imageUrl", out var img))
                {
                    imageUrl = img.GetString();
                }

                list.Add(new Representative
                {
                    BioguideId = bioguideId,
                    Name = name,
                    Party = partyName,
                    StateCode = stateCode,   // 2-letter code
                    StateName = stateName,   // full name
                    DistrictNumber = districtNumber,
                    Chamber = chamber,
                    StartYear = startYear,
                    ImageUrl = imageUrl,
                    LastUpdatedUtc = now
                });
            }

            return list;
        }
    }
}
