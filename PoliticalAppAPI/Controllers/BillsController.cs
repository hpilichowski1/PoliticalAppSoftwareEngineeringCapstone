using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PoliticalAppAPI.DTOs.Common;
using PoliticalAppAPI.DTOs.Bills;
using PoliticalAppAPI.Services;
using PoliticalAppAPI.Data;
using PoliticalAppAPI.Models;

namespace PoliticalAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillsController : ControllerBase
    {
        private readonly IBillSyncService _billSync;
        private readonly AppDbContext _db;

        public BillsController(IBillSyncService billSync, AppDbContext db)
        {
            _billSync = billSync;
            _db = db;
        }

        // GET api/bills?page=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<PagedResult<BillDto>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var currentUser = await GetCurrentUserAsync();
            var currentUserId = currentUser?.UserId;

            var query = _db.Bills
                .Include(b => b.Votes)
                .OrderByDescending(b => b.LatestActionDate);

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BillDto
                {
                    Id = b.Id,
                    Congress = b.Congress,
                    BillType = b.BillType,
                    BillNumber = b.BillNumber,
                    Title = b.Title,
                    LatestActionDate = b.LatestActionDate,
                    LatestActionText = b.LatestActionText,
                    SummaryText = b.SummaryText,
                    UpVotes = b.Votes.Count(v => v.Vote == VoteType.Up),
                    DownVotes = b.Votes.Count(v => v.Vote == VoteType.Down),

                    // 🔑 This is what tells MAUI which color to use on app start
                    UserVote = currentUserId == null
                        ? null
                        : b.Votes
                            .Where(v => v.UserId == currentUserId)
                            .Select(v => (VoteType?)v.Vote)
                            .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(new PagedResponse<BillDto>
            {
                Items = items,
                Total = total
            });
        }

        // ----- Voting -----

        public class VoteRequest
        {
            public VoteType Vote { get; set; }
            public string UserIdentifier { get; set; } = string.Empty;
        }

        [HttpPost("{billId}/vote")]
        public async Task<ActionResult<BillDto>> Vote(
            int billId,
            [FromBody] VoteRequest request)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
                return BadRequest("User not found for current header email.");

            var bill = await _db.Bills
                .Include(b => b.Votes)
                .FirstOrDefaultAsync(b => b.Id == billId);

            if (bill == null)
                return NotFound();

            var existing = bill.Votes
                .FirstOrDefault(v => v.UserId == currentUser.UserId);

            if (request.Vote == VoteType.None)
            {
                if (existing != null)
                    _db.BillVotes.Remove(existing);
            }
            else
            {
                if (existing == null)
                {
                    bill.Votes.Add(new BillVote
                    {
                        UserId = currentUser.UserId,
                        Vote = request.Vote
                    });
                }
                else
                {
                    existing.Vote = request.Vote;
                }
            }

            await _db.SaveChangesAsync();

            // Rebuild DTO using the same logic as in GET
            var dto = new BillDto
            {
                Id = bill.Id,
                Congress = bill.Congress,
                BillType = bill.BillType,
                BillNumber = bill.BillNumber,
                Title = bill.Title,
                LatestActionDate = bill.LatestActionDate,
                LatestActionText = bill.LatestActionText,
                SummaryText = bill.SummaryText,
                UpVotes = bill.Votes.Count(v => v.Vote == VoteType.Up),
                DownVotes = bill.Votes.Count(v => v.Vote == VoteType.Down),
                UserVote = bill.Votes
                    .Where(v => v.UserId == currentUser.UserId)
                    .Select(v => (VoteType?)v.Vote)
                    .FirstOrDefault()
            };

            return Ok(dto);
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            // This must match the header name you add in MAUI
            var email = Request.Headers["X-User-Email"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
