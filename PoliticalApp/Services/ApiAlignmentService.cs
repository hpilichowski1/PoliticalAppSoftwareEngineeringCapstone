using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PoliticalApp.Models;

namespace PoliticalApp.Services
{
    public class ApiAlignmentService : IAlignmentService
    {
        private readonly HttpClient _http;

        public ApiAlignmentService(HttpClient http)
        {
            _http = http;
        }

        public async Task<int?> GetMyScoreAsync()
        {
            var result = await _http.GetFromJsonAsync<AlignmentScoreResponse>("api/alignment/me");
            return result?.Score;
        }

        private void EnsureUserHeader()
        {
            _http.DefaultRequestHeaders.Remove("X-User-Email");
            if (!string.IsNullOrWhiteSpace(App.CurrentUsername))
            {
                _http.DefaultRequestHeaders.Add("X-User-Email", App.CurrentUsername);
            }
        }

        public async Task<bool> SubmitScoreAsync(int score)
        {
            EnsureUserHeader();

            var resp = await _http.PostAsJsonAsync("api/alignment/submit", new { Score = score });
            return resp.IsSuccessStatusCode;
        }

        class AlignmentScoreResponse
        {
            public int? Score { get; set; }
        }
    }
}