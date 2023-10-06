using CotdQualifierRankWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CotdQualifierRankWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankController : ControllerBase
    {
        CotdContext _context { get; set; }

        public RankController(CotdContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("{mapUID}/{time:int}")]
        public IActionResult GetAction(string mapUID, int time)
        {
            var cotd = _context.Competitions.Include(c => c.Leaderboard).Where(c => c.NadeoMapUid == mapUID).FirstOrDefault();

            if (cotd == null || cotd.Leaderboard == null)
            {
                return NotFound();
            }

            // binary search on the leaderboard to find the rank as if it would have been at the correct location in the list
            cotd.Leaderboard.Sort((a, b) => a.Time.CompareTo(b.Time));
            int rank = 0;
            int min = 0;
            int max = cotd.Leaderboard.Count;
            while (min < max)
            {
                int mid = (min + max) / 2;
                if (cotd.Leaderboard[mid].Time < time)
                {
                    min = mid + 1;
                }
                else
                {
                    max = mid;
                }
            }
            rank = min + 1;

            return Ok(new
            {
                mapUID,
                competitionId = cotd.NadeoCompetitionId,
                challengeId = cotd.NadeoChallengeId,
                date = cotd.Date,
                time,
                rank,
            });
        }
    }
}
