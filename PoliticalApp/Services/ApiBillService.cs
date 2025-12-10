using System.Net.Http.Json;
using System.Text.Json;
using PoliticalApp.Models;
using System.Diagnostics;

namespace PoliticalApp.Services
{
    public class ApiBillService : IBillService
    {
        private readonly HttpClient _http;

        private class PagedResult<T>
        {
            public List<T> Items { get; set; } = new();
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int Total { get; set; }
        }

        public ApiBillService(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<IReadOnlyList<Bill>> GetBillsAsync(int page, int pageSize)
        {
            EnsureUserHeader();
            var url = $"api/bills?page={page}&pageSize={pageSize}";

            // Case-insensitive so camelCase JSON maps to PascalCase properties
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = await _http.GetFromJsonAsync<PagedResult<Bill>>(url, options);

            if (result == null || result.Items == null)
                return Array.Empty<Bill>();

            return result.Items;
        }

        private void EnsureUserHeader()
        {
            _http.DefaultRequestHeaders.Remove("X-User-Email");
            if (!string.IsNullOrWhiteSpace(App.CurrentUsername))
            {
                _http.DefaultRequestHeaders.Add("X-User-Email", App.CurrentUsername);
            }
        }

        public async Task<Bill?> VoteAsync(int billId, VoteType vote)
        {
            EnsureUserHeader();

            Debug.WriteLine($"Set App.CurrentUsername = {App.CurrentUsername}");

            var body = new
            {
                vote,
                userIdentifier = App.CurrentUsername // 👈 username used by backend
            };

            var response = await _http.PostAsJsonAsync($"api/bills/{billId}/vote", body);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<Bill>();
        }
    }
}
