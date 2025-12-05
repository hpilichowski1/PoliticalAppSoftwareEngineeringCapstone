using Microsoft.AspNetCore.Mvc;
using PoliticalAppAPI.DTOs;
using PoliticalAppAPI.DTOs.Bills;
using PoliticalAppAPI.Services;
using PoliticalAppAPI.DTOs.Common;

namespace PoliticalAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillsController : ControllerBase
    {
        private readonly IBillSyncService _billSync;

        public BillsController(IBillSyncService billSync)
        {
            _billSync = billSync;
        }

        // GET api/bills?page=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<PagedResult<BillDto>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (bills, total) = await _billSync.GetPagedAsync(page, pageSize);

            var dtos = bills.Select(b => new BillDto
            {
                Id = b.Id,
                Congress = b.Congress,
                BillType = b.BillType,
                BillNumber = b.BillNumber,
                Title = b.Title,
                PolicyArea = b.PolicyArea,
                SponsorName = b.SponsorName,
                LatestActionDate = b.LatestActionDate,
                LatestActionText = b.LatestActionText,
                SummaryText = b.SummaryText
            }).ToList();

            var result = new PagedResult<BillDto>
            {
                Items = dtos,
                Page = page,
                PageSize = pageSize,
                Total = total
            };

            return Ok(result);
        }
    }
}
