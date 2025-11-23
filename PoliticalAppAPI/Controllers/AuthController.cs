using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PoliticalAppAPI.DTOs.Auth;
using PoliticalAppAPI.Models;
using PoliticalAppAPI.Data;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return Unauthorized(new LoginResponse
            {
                Success = false,
                Message = "Invalid email or password."
            });
        }

        bool passwordMatches = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!passwordMatches)
        {
            return Unauthorized(new LoginResponse
            {
                Success = false,
                Message = "Invalid email or password."
            });
        }

        return Ok(new LoginResponse
        {
            Success = true,
            Message = "Login successful.",
            UserId = user.UserId,
            Name = user.Name,
            Role = user.Role,
            Email = user.Email
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
    {
        // 1. Basic validation
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new LoginResponse
            {
                Success = false,
                Message = "Name, email, and password are required."
            });
        }

        // 2. Check if email already exists
        var existing = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existing != null)
        {
            return Conflict(new LoginResponse
            {
                Success = false,
                Message = "An account with this email already exists."
            });
        }

        // 3. Create user with hashed password
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Role = "citizen",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // 4. Return same shape as login
        return Ok(new LoginResponse
        {
            Success = true,
            Message = "Registration successful.",
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        });
    }
}