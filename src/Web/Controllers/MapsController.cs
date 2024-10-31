using CotdQualifierRank.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CotdQualifierRank.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MapsController(CompetitionService competitionService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUids()
    {
        var maps = competitionService.GetMapsUids();

        if (maps.Count == 0)
            return NotFound();

        return Ok(maps);
    }
}