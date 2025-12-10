using System.Net.Http.Json;
using System.Text.Json;
using PoliticalApp.Models;
using System.Diagnostics;

namespace PoliticalApp.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient httpClient)
    {
        _http = httpClient;  // this is the one configured in MauiProgram
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        Debug.WriteLine($"[LoginAsync] called with email = '{email}'");

        var req = new LoginRequest { Email = email, Password = password };

        HttpResponseMessage resp;
        try
        {
            resp = await _http.PostAsJsonAsync("api/auth/login", req);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[LoginAsync] HTTP error: {ex.Message}");
            return new LoginResponse
            {
                Success = false,
                Message = "Network or HTTP error: " + ex.Message
            };
        }

        Debug.WriteLine($"[LoginAsync] HTTP {(int)resp.StatusCode} {resp.StatusCode}");

        var raw = await resp.Content.ReadAsStringAsync();
        Debug.WriteLine($"[LoginAsync] Raw response: {raw}");

        if (!resp.IsSuccessStatusCode)
        {
            // We never get to set App.CurrentUsername here
            return new LoginResponse
            {
                Success = false,
                Message = $"HTTP {(int)resp.StatusCode} {resp.StatusCode}: {raw}"
            };
        }

        // Only if HTTP status is 2xx do we set the username
        App.CurrentUsername = email;
        Debug.WriteLine($"[LoginAsync] Set App.CurrentUsername = '{App.CurrentUsername}'");

        try
        {
            var result = JsonSerializer.Deserialize<LoginResponse>(raw,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Server returned empty response."
                };
            }

            return result;
        }
        catch (Exception ex)
        {
            return new LoginResponse
            {
                Success = false,
                Message = $"Invalid JSON from server: {ex.Message}. Raw: {raw}"
            };
        }
    }


    public async Task<LoginResponse> RegisterAsync(string name, string email, string password)
    {
        var req = new RegisterRequest
        {
            Name = name,
            Email = email,
            Password = password
        };

        HttpResponseMessage resp;

        try
        {
            resp = await _http.PostAsJsonAsync("api/auth/register", req);
        }
        catch (Exception ex)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "HTTP error (no response): " + ex.Message
            };
        }

        var raw = await resp.Content.ReadAsStringAsync();

        // If not 2xx, don't try to parse as JSON, just show status + raw body
        if (!resp.IsSuccessStatusCode)
        {
            return new LoginResponse
            {
                Success = false,
                Message = $"HTTP {(int)resp.StatusCode} {resp.StatusCode}: {raw}"
            };
        }

        try
        {
            var result = JsonSerializer.Deserialize<LoginResponse>(raw,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Server returned empty response."
                };
            }

            return result;
        }
        catch (Exception ex)
        {
            return new LoginResponse
            {
                Success = false,
                Message = $"Invalid JSON from server: {ex.Message}. Raw: {raw}"
            };
        }
    }
}
