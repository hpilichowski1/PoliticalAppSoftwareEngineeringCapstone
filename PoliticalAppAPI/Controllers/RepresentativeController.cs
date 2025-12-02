using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PoliticalAppAPI.DTOs.Reps;
using PoliticalAppAPI.Models;
using PoliticalAppAPI.Services;

namespace PoliticalAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepresentativesController : ControllerBase
    {
        private readonly IRepresentativeSyncService _syncService;
        private readonly ILogger<RepresentativesController> _logger;

        public RepresentativesController(
            IRepresentativeSyncService syncService,
            ILogger<RepresentativesController> logger)
        {
            _syncService = syncService;
            _logger = logger;
        }

        // GET api/representatives?state=FL
        // GET api/representatives           -> all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RepresentativeDto>>> Get(
            [FromQuery] string? state = null)
        {
            try
            {
                var reps = string.IsNullOrWhiteSpace(state)
                    ? await _syncService.GetOrRefreshAllAsync()
                    : await _syncService.GetOrRefreshByStateAsync(state);

                var nowYear = DateTime.UtcNow.Year;

                var dto = reps.Select(r =>
                {
                    var title = r.Chamber == "Senate" ? "Senator" : "Representative";
                    var districtLabel = r.DistrictNumber.HasValue
                        ? $"{r.StateCode}-{r.DistrictNumber}"
                        : r.StateCode;

                    return new RepresentativeDto
                    {
                        Name = r.Name,
                        Title = title,
                        Party = MapParty(r.Party),
                        District = districtLabel,
                        Bio = $"{title} for {districtLabel}",
                        ConsistencyScore = 0.0,
                        YearsInOffice = r.StartYear > 0 ? nowYear - r.StartYear : 0,
                        Level = "Federal"
                    };
                }).ToList();

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in GET /api/representatives");
                return StatusCode(500, "Internal server error.");
            }
        }

        private static string MapParty(string? party)
        {
            if (string.IsNullOrWhiteSpace(party)) return "Unknown";
            var p = party.Trim();

            return p switch
            {
                "R" => "Republican",
                "D" => "Democrat",
                "I" => "Independent",
                _ => p
            };
        }
    }
}
