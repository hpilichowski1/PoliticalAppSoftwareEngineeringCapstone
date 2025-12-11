using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PoliticalAppAPI.DTOs.Auth;
using PoliticalAppAPI.Models;
using PoliticalAppAPI.Data;

[ApiController]
[Route("api/alignment")]
public class AlignmentController : ControllerBase
{
    private readonly AppDbContext _db;

    public AlignmentController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitScore([FromBody] AlignmentSubmitRequest req)
    {
        // Identify user by email header
        var email = Request.Headers["X-User-Email"].ToString();
        if (string.IsNullOrWhiteSpace(email))
            return Unauthorized();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return Unauthorized();

        user.AlignmentScore = req.Score;
        await _db.SaveChangesAsync();

        return Ok(new { success = true, score = req.Score });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyScore()
    {
        var email = Request.Headers["X-User-Email"].ToString();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        return Ok(new { score = user?.AlignmentScore });
    }
}

public class AlignmentSubmitRequest
{
    public int Score { get; set; }
}
