namespace PoliticalAppAPI.DTOs
{
    public record RegisterRequest(string Name, string Email, string Password);
    public record LoginRequest(string Email, string Password);
    public record AuthResponse(string UserId, string Name, string Email, string Role);
}
