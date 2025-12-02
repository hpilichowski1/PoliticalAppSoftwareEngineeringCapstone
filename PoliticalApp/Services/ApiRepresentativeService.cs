using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PoliticalApp.Models;

namespace PoliticalApp.Services
{
    public class ApiRepresentativeService : IRepresentativeService
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions JsonOpts =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public ApiRepresentativeService(HttpClient http)
        {
            _http = http;
            if (_http.BaseAddress == null)
            {
                // TODO: adjust to your actual API base URL / port
                _http.BaseAddress = new Uri("http://localhost:5154");
            }
        }

        public async Task<List<Representative>> GetRepresentativesAsync(
            string? stateCode = null,
            string? level = null,
            string? search = null)
        {
            var query = string.IsNullOrWhiteSpace(stateCode)
                ? "api/representatives"
                : $"api/representatives?state={stateCode.Trim().ToUpper()}";

            var response = await _http.GetAsync(query);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var reps = JsonSerializer.Deserialize<List<Representative>>(json, JsonOpts)
                       ?? new List<Representative>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                reps = reps.Where(r =>
                        r.Name.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                        r.District.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                        r.Title.Contains(s, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // level can be used later if you add different endpoints
            return reps;
        }
    }
}
