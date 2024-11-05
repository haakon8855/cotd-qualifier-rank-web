using CotdQualifierRank.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CotdQualifierRank.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MapsController(CompetitionService competitionService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<string[]> GetUids()
    {
        var maps = competitionService.GetMapsUids();

        if (!maps.Any())
            return NotFound();

        return Ok(maps);
    }
}