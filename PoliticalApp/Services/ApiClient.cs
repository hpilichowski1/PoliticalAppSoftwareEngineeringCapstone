using System.Net.Http.Json;
using PoliticalApp.Models;

namespace PoliticalApp.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient httpClient)
    {
        _http = httpClient;
        _http.BaseAddress = new Uri("https://localhost:5001/"); // modify later for Android/iOS
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var req = new LoginRequest { Email = email, Password = password };

        var response = await _http.PostAsJsonAsync("api/auth/login", req);

        if (!response.IsSuccessStatusCode)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Login failed."
            };
        }

        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public async Task<LoginResponse?> RegisterAsync(string name, string email, string password)
    {
        var req = new RegisterRequest
        {
            Name = name,
            Email = email,
            Password = password
        };

        var response = await _http.PostAsJsonAsync("api/auth/register", req);

        if (!response.IsSuccessStatusCode)
        {
            // Try read message if the API sent one
            var error = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return error ?? new LoginResponse
            {
                Success = false,
                Message = "Registration failed."
            };
        }

        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }
}
