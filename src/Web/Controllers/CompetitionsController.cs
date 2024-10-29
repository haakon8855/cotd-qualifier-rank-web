using CotdQualifierRank.Web.DTOs;
using CotdQualifierRank.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CotdQualifierRank.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompetitionsController : ControllerBase
{
    private CompetitionService _competitionService { get; set; }

    public CompetitionsController(CompetitionService competitionService)
    {
        _competitionService = competitionService;
    }

    [HttpGet("{mapUid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompetitionDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompetitionByMap(string mapUid)
    {
        var competition = await _competitionService.GetCompetitionByMapUid(mapUid, false);

        if (competition is null)
        {
            return NotFound();
        }

        return Ok(new CompetitionDTO
        {
            ChallengeId = competition.NadeoChallengeId,
            CompetitionId = competition.NadeoCompetitionId,
            Date = competition.Date,
            MapUid = competition.NadeoMapUid,
        });
    }

    [HttpGet("{competitionId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompetitionDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompetitionByCompetitionId(int competitionId)
    {
        var competition = await _competitionService.GetCompetition(competitionId, false);

        if (competition is null)
        {
            return NotFound();
        }

        return Ok(new CompetitionDTO
        {
            ChallengeId = competition.NadeoChallengeId,
            CompetitionId = competition.NadeoCompetitionId,
            Date = competition.Date,
            MapUid = competition.NadeoMapUid,
        });
    }

    [HttpGet("{mapUid}/leaderboard")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<int>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompetitionLeaderboardByMapUid(string mapUid)
    {
        var competition = await _competitionService.GetCompetitionByMapUid(mapUid, true);

        if (competition is null || competition.Leaderboard is null)
        {
            return NotFound();
        }

        var hei = Guid.NewGuid();

        var leaderboard = competition.Leaderboard.OrderBy(r => r.Time).Select(r => r.Time);
        return Ok(leaderboard);
    }

    [HttpGet("{competitionId:int}/leaderboard")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<int>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompetitionLeaderboardByChallengeId(int competitionId)
    {
        var competition = await _competitionService.GetCompetition(competitionId, true);

        if (competition is null || competition.Leaderboard is null)
        {
            return NotFound();
        }

        var leaderboard = competition.Leaderboard.OrderBy(r => r.Time).Select(r => r.Time);
        return Ok(leaderboard);
    }
}