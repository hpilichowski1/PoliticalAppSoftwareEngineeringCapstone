using System.Net.Http.Json;
using System.Diagnostics;
using PoliticalApp.Models;

namespace PoliticalApp.Services
{
    public class ApiProfileService : IProfileService
    {
        private readonly HttpClient _http;

        public ApiProfileService(HttpClient http)
        {
            _http = http;
        }

        private void ApplyUserHeader()
        {
            _http.DefaultRequestHeaders.Remove("X-User-Email");

            if (!string.IsNullOrWhiteSpace(App.CurrentUsername))
            {
                _http.DefaultRequestHeaders.Add("X-User-Email", App.CurrentUsername);
                Debug.WriteLine($"[ApiProfileService] X-User-Email = {App.CurrentUsername}");
            }
            else
            {
                Debug.WriteLine("[ApiProfileService] App.CurrentUsername is empty");
            }
        }

        public async Task<ProfileInfo?> GetProfileAsync()
        {
            ApplyUserHeader();
            return await _http.GetFromJsonAsync<ProfileInfo>("api/profile/me");
        }

        public async Task<bool> UpdateLocationAsync(string state, string region)
        {
            ApplyUserHeader();

            var body = new { State = state, Region = region };
            var resp = await _http.PutAsJsonAsync("api/profile/location", body);
            return resp.IsSuccessStatusCode;
        }
    }
}