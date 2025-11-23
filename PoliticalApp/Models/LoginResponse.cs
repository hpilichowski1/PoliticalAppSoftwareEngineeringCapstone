namespace PoliticalApp.Models
{
    public class LoginResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? UserId { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }
    public string? Email { get; set; }
}
}