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
        NadeoApiController _nadeoApiController { get; set; }

        public RankController(CotdContext context, NadeoApiController nadeoApiController)
        {
            _context = context;
            _nadeoApiController = nadeoApiController;
        }

        [HttpGet]
        [Route("{mapUID}/{time:int}")]
        public async Task<IActionResult> GetAction(string mapUid, int time)
        {
            var cotd = _context.Competitions.Include(c => c.Leaderboard).FirstOrDefault(c => c.NadeoMapUid == mapUid);

            if (cotd == null)
            {
                // Need to fetch data from nadeo


                var response = await _nadeoApiController.GetTodtInfoFromMap(mapUid);
                if (response != null)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
            }

            if (cotd.Leaderboard == null)
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
                mapUid,
                competitionId = cotd.NadeoCompetitionId,
                challengeId = cotd.NadeoChallengeId,
                date = cotd.Date,
                time,
                rank,
            });
        }
    }
}
