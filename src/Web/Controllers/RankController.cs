using CotdQualifierRank.Database.Models;
using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Web.DTOs;
using CotdQualifierRank.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CotdQualifierRank.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RankController(RankService rankService) : ControllerBase
{
    private const int RequestedTimeout = 5;

    private static string TimeoutMessage => "Requested map is either not a valid TOTD or the COTD " +
                                     "leaderboard is currently being fetched from " +
                                     "Nadeo and will be available shortly. " +
                                     "Please retry in " + RequestedTimeout + " seconds.";

    [HttpGet("{mapUid}/{time:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RankDTO))]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult GetQualifierRank(string mapUid, int time)
    {
        if (!MapUid.IsValid(mapUid))
            return BadRequest("Requested mapUid is not valid");

        var rankDTO = rankService.GetRank(new MapUid(mapUid), time);

        if (rankDTO is null)
        {
            Response.Headers.Append("Retry-After", RequestedTimeout.ToString());
            return StatusCode(503, new { message = TimeoutMessage });
        }

        return Ok(rankDTO);
    }
}