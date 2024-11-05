using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Domain.DomainPrimitives.Nadeo;
using CotdQualifierRank.Web.DTOs;
using CotdQualifierRank.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CotdQualifierRank.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompetitionsController(CompetitionService competitionService) : ControllerBase
{
    [HttpGet("{mapUid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CompetitionDTO> GetCompetitionByMap(string mapUid)
    {
        if (!MapUid.IsValid(mapUid))
            return BadRequest("Requested mapUid is not valid");
                
        var competitionDTO = competitionService.GetCompetitionDTO(new MapUid(mapUid), false);

        if (competitionDTO is null)
            return NotFound();

        return Ok(competitionDTO);
    }

    [HttpGet("{competitionId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CompetitionDTO> GetCompetitionByCompetitionId(int competitionId)
    {
        if (!NadeoCompetitionId.IsValid(competitionId))
            return BadRequest("Requested competitionId is not valid");
        
        var competition = competitionService.GetCompetitionDTO(new NadeoCompetitionId(competitionId), false);

        if (competition is null)
            return NotFound();

        return Ok(competition);
    }

    [HttpGet("{mapUid}/leaderboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<int>> GetCompetitionLeaderboardByMapUid(string mapUid)
    {
        if (!MapUid.IsValid(mapUid))
            return BadRequest("Requested mapUid is not valid");
                
        return Ok(competitionService.GetLeaderboard(new MapUid(mapUid)));
    }

    [HttpGet("{competitionId:int}/leaderboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<int>> GetCompetitionLeaderboardByChallengeId(int competitionId)
    {
        if (!NadeoCompetitionId.IsValid(competitionId))
            return BadRequest("Requested competitionId is not valid");
        
        return Ok(competitionService.GetLeaderboard(new NadeoCompetitionId(competitionId)));
    }
}