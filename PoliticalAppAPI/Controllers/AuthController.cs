using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PoliticalAppAPI.Data;
using PoliticalAppAPI.DTOs;
using PoliticalAppAPI.Models;

namespace PoliticalAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
    {
        // basic uniqueness check
        var exists = await _db.Users.AnyAsync(u => u.Email == req.Email);
        if (exists) return Conflict("Email is already registered.");

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Name = req.Name.Trim(),
            Email = req.Email.Trim().ToLowerInvariant(),
            Role = "citizen",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new AuthResponse(user.UserId, user.Name, user.Email, user.Role);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
    {
        var email = req.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null || string.IsNullOrEmpty(user.PasswordHash))
            return Unauthorized("Invalid credentials.");

        var ok = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
        if (!ok) return Unauthorized("Invalid credentials.");

        return new AuthResponse(user.UserId, user.Name, user.Email, user.Role);
    }
}
