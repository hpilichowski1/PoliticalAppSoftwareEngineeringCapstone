using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PoliticalApp.Models;

namespace PoliticalApp.Services
{
    public interface IRepresentativeService
    {
        // Used internally (or directly if you like)
        Task<List<Representative>> GetMembersByStateAsync(string stateCode);

        // This is what your HomeViewModel is probably calling:
        // _repService.GetRepresentativesAsync(stateFilter, levelFilter, searchText)
        Task<List<Representative>> GetRepresentativesAsync(
            string? stateCode = null,
            string? level = null,
            string? search = null);
    }

    public class RepresentativeService : IRepresentativeService
    {
        private readonly HttpClient _httpClient;

        // TODO: move this into secure config / user secrets
        private const string ApiKey = "EtW0DpIKAgSaVZHe4IubWtKbmcYwgXkVOSGZwkhC";

        public RepresentativeService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("https://api.congress.gov");
            }

            // Use header style as recommended in docs
            if (!_httpClient.DefaultRequestHeaders.Contains("X-API-Key"))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", ApiKey);
            }
        }

        /// <summary>
        /// Returns all current members for a given state using /v3/member/{state}?limit=250
        /// </summary>
        public async Task<List<Representative>> GetMembersByStateAsync(string stateCode)
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
                throw new InvalidOperationException("Congress.gov API key is not configured.");

            stateCode = stateCode.Trim().ToUpper();

            // 👇 Match documented pattern: add api_key, format, currentMember
            var url = $"/v3/member/{stateCode}?limit=250&format=json&currentMember=true&api_key={ApiKey}";

            var reps = new List<Representative>();

            try
            {
                using var response = await _httpClient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"[Congress API] URL: {url}");
                System.Diagnostics.Debug.WriteLine($"[Congress API] Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[Congress API] Body: {json}");

                response.EnsureSuccessStatusCode();

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("members", out var membersElement) ||
                    membersElement.ValueKind != JsonValueKind.Array)
                {
                    System.Diagnostics.Debug.WriteLine("[Congress API] No 'members' array in response");
                    return reps;
                }

                foreach (var m in membersElement.EnumerateArray())
                {
                    string name = m.TryGetProperty("name", out var nameEl)
                        ? nameEl.GetString() ?? string.Empty
                        : string.Empty;

                    string partyName = m.TryGetProperty("partyName", out var partyEl)
                        ? partyEl.GetString() ?? string.Empty
                        : string.Empty;

                    string state = m.TryGetProperty("state", out var stateEl)
                        ? stateEl.GetString() ?? stateCode
                        : stateCode;

                    string district = string.Empty;
                    if (m.TryGetProperty("district", out var distEl))
                    {
                        if (distEl.ValueKind == JsonValueKind.String)
                        {
                            district = distEl.GetString() ?? string.Empty;
                        }
                        else if (distEl.ValueKind == JsonValueKind.Number && distEl.TryGetInt32(out var dNum))
                        {
                            district = dNum.ToString();
                        }
                    }

                    string role = m.TryGetProperty("role", out var roleEl)
                        ? roleEl.GetString() ?? string.Empty
                        : string.Empty;

                    int yearsInOffice = 0;
                    if (m.TryGetProperty("startDate", out var startDateEl) &&
                        startDateEl.ValueKind == JsonValueKind.String &&
                        DateTime.TryParse(startDateEl.GetString(), out var start))
                    {
                        var now = DateTime.UtcNow;
                        yearsInOffice = Math.Max(0, (int)((now - start).TotalDays / 365.25));
                    }

                    var districtLabel = string.IsNullOrWhiteSpace(district)
                        ? state
                        : $"{state}-{district}";

                    var title = role;
                    var bio = $"{title} for {districtLabel}";

                    reps.Add(new Representative
                    {
                        Name = name,
                        Title = title,
                        Party = MapParty(partyName),
                        District = districtLabel,
                        Bio = bio,
                        ConsistencyScore = 0.0,
                        YearsInOffice = yearsInOffice,
                        Level = "State"
                    });
                }

                System.Diagnostics.Debug.WriteLine($"[Congress API] Parsed {reps.Count} reps for {stateCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Congress API] Error: {ex}");
            }

            return reps;
        }


        // This is the method HomeViewModel is calling
        public async Task<List<Representative>> GetRepresentativesAsync(
            string? stateCode = null,
            string? level = null,
            string? search = null)
        {
            List<Representative> reps;

            if (string.IsNullOrWhiteSpace(stateCode))
            {
                // ✅ No state passed → get ALL current members
                reps = await GetAllCurrentMembersAsync();
            }
            else
            {
                var state = stateCode.Trim().ToUpperInvariant();
                reps = await GetMembersByStateAsync(state);
            }

            // Optional level filter ("Federal", "State", etc.)
            if (!string.IsNullOrWhiteSpace(level) &&
                !string.Equals(level, "All", StringComparison.OrdinalIgnoreCase))
            {
                reps = reps.FindAll(r =>
                    !string.IsNullOrEmpty(r.Level) &&
                    string.Equals(r.Level, level, StringComparison.OrdinalIgnoreCase));
            }

            // Optional search by name / title / district
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();

                reps = reps.FindAll(r =>
                    (!string.IsNullOrEmpty(r.Name) &&
                    r.Name.Contains(s, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(r.Title) &&
                    r.Title.Contains(s, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(r.District) &&
                    r.District.Contains(s, StringComparison.OrdinalIgnoreCase)));
            }

            return reps;
        }


        private async Task<List<Representative>> GetAllCurrentMembersAsync()
        {
            // All current members, any state
            var url = "/v3/member?currentMember=true&limit=250&format=json";

            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var reps = new List<Representative>();

            if (!root.TryGetProperty("members", out var membersElement) ||
                membersElement.ValueKind != JsonValueKind.Array)
            {
                return reps;
            }

            foreach (var m in membersElement.EnumerateArray())
            {
                string name = m.TryGetProperty("name", out var nameEl)
                    ? nameEl.GetString() ?? string.Empty
                    : string.Empty;

                string partyName = m.TryGetProperty("partyName", out var partyEl)
                    ? partyEl.GetString() ?? string.Empty
                    : string.Empty;

                string state = m.TryGetProperty("state", out var stateEl)
                    ? stateEl.GetString() ?? string.Empty
                    : string.Empty;

                string district = string.Empty;
                if (m.TryGetProperty("district", out var distEl))
                {
                    if (distEl.ValueKind == JsonValueKind.String)
                    {
                        district = distEl.GetString() ?? string.Empty;
                    }
                    else if (distEl.ValueKind == JsonValueKind.Number && distEl.TryGetInt32(out var dNum))
                    {
                        district = dNum.ToString();
                    }
                }

                string role = m.TryGetProperty("role", out var roleEl)
                    ? roleEl.GetString() ?? string.Empty
                    : string.Empty;

                int yearsInOffice = 0;
                if (m.TryGetProperty("startDate", out var startDateEl) &&
                    startDateEl.ValueKind == JsonValueKind.String &&
                    DateTime.TryParse(startDateEl.GetString(), out var start))
                {
                    var now = DateTime.UtcNow;
                    yearsInOffice = Math.Max(0, (int)((now - start).TotalDays / 365.25));
                }

                var districtLabel = string.IsNullOrWhiteSpace(district)
                    ? state
                    : $"{state}-{district}";

                var title = role;
                var bio = $"{title} for {districtLabel}";

                reps.Add(new Representative
                {
                    Name = name,
                    Title = title,
                    Party = MapParty(partyName),
                    District = districtLabel,
                    Bio = bio,
                    ConsistencyScore = 0.0,
                    YearsInOffice = yearsInOffice,
                    Level = "Federal" // or whatever you decided earlier
                });
            }

            return reps;
        }


        private static string MapParty(string? partyName)
        {
            if (string.IsNullOrWhiteSpace(partyName))
                return "Unknown";

            var p = partyName.Trim();

            return p switch
            {
                "R" => "Republican",
                "D" => "Democrat",
                "I" => "Independent",
                _ => p     // if already full text (e.g. "Republican"), just return
            };
        }
    }
}
