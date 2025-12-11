using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PoliticalAppAPI.DTOs.Profile;
using PoliticalAppAPI.Data;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(AppDbContext db, ILogger<ProfileController> logger)
    {
        _db = db;
        _logger = logger;
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        var email = Request.Headers["X-User-Email"].FirstOrDefault();
        _logger.LogInformation("Profile: X-User-Email = {Email}", email ?? "<none>");

        if (string.IsNullOrWhiteSpace(email))
            return null;

        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    // GET api/profile/me
    [HttpGet("me")]
    public async Task<ActionResult<ProfileDto>> GetMe()
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return Unauthorized("User not found for provided email.");

        // join votes + bills
        var votes = await _db.BillVotes
            .Where(v => v.UserId == user.UserId)
            .Include(v => v.Bill)
            .OrderByDescending(v => v.CreatedUtc)
            .Select(v => new UserVoteDto
            {
                BillId = v.BillId,
                BillTitle = v.Bill.Title,
                Vote = v.Vote
            })
            .ToListAsync();

        var dto = new ProfileDto
        {
            Email = user.Email,
            Name = user.Name,
            State = user.State,
            Region = user.Region,
            Votes = votes,
            AlignmentScore = user.AlignmentScore
        };

        return Ok(dto);
    }

    // PUT api/profile/location
    [HttpPut("location")]
    public async Task<ActionResult> UpdateLocation([FromBody] UpdateLocationRequest request)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return Unauthorized("User not found for provided email.");

        user.State = request.State;
        user.Region = request.Region;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
