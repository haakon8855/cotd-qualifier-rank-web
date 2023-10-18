using CotdQualifierRankWeb.DTOs;
using CotdQualifierRankWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace CotdQualifierRankWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompetitionsController : ControllerBase
    {
        private CompetitionService _competitionService { get; set; }

        public CompetitionsController(CompetitionService competitionService)
        {
            _competitionService = competitionService;
        }

        [HttpGet]
        [Route("{mapUID}")]
        public IActionResult GetCompetitionByMap(string mapUid)
        {
            var competition = _competitionService.GetCompetitionByMapUid(mapUid, false);

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

        //[HttpGet]
        //[Route("{mapUID}/leaderboard")]

        [HttpGet]
        [Route("{competitionId:int}")]
        public IActionResult GetCompetitionByCompetitionId(int competitionId)
        {
            var competition = _competitionService.GetCompetition(competitionId);

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
    }
}
