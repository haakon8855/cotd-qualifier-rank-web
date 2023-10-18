using CotdQualifierRankWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace CotdQualifierRankWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapsController : ControllerBase
    {
        private CompetitionService _competitionService { get; set; }

        public MapsController(CompetitionService competitionService)
        {
            _competitionService = competitionService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMapUids()
        {
            var maps = await _competitionService.GetMapsUids();

            if (maps is null)
            {
                return NotFound();
            }

            return Ok(maps);
        }
    }
}
