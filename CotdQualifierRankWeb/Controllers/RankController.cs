using CotdQualifierRankWeb.Data;
using CotdQualifierRankWeb.DTOs;
using CotdQualifierRankWeb.Models;
using CotdQualifierRankWeb.Utils;
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

        [HttpGet("{mapUid}/{time:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RankDTO))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public IActionResult GetQualifierRank(string mapUid, int time)
        {
            // check if mapUid matches pattern
            if (!Competition.IsValidMapUid(mapUid))
            {
                int secondsToWait = 5;
                Response.Headers.Add("Retry-After", secondsToWait.ToString());
                return StatusCode(503,
                    new
                    {
                        message = "Requested map is either not a valid TOTD or the COTD " +
                                  "leaderboard is currently being fetched from " +
                                  "Nadeo and will be available shortly. " +
                                  "Please retry in " + secondsToWait.ToString() + " seconds."
                    }
                );
            }

            var cotd = _context.Competitions.Include(c => c.Leaderboard).FirstOrDefault(c => c.NadeoMapUid == mapUid);

            if (cotd is null)
            {
                if (!QueueService.QueueContains(mapUid))
                {
                    QueueService.AddToQueue(mapUid);
                }

                int secondsToWait = 5;
                Response.Headers.Add("Retry-After", secondsToWait.ToString());
                return StatusCode(503,
                    new
                    {
                        message = "Requested map is either not a valid TOTD or the COTD " +
                                  "leaderboard is currently being fetched from " +
                                  "Nadeo and will be available shortly. " +
                                  "Please retry in " + secondsToWait.ToString() + " seconds."
                    }
                );
            }

            var rank = FindRankInLeaderboard(cotd, time);

            return Ok(new RankDTO
            {
                MapUid = mapUid,
                CompetitionId = cotd.NadeoCompetitionId,
                ChallengeId = cotd.NadeoChallengeId,
                Date = cotd.Date,
                Time = time,
                Rank = rank,
                PlayerCount = cotd.Leaderboard?.Count ?? 0,
                LeaderboardIsEmpty = cotd.Leaderboard is null || cotd.Leaderboard.Count == 0,
            });
        }

        [NonAction]
        public int FindRankInLeaderboard(Competition cotd, int time)
        {
            // Binary search on the leaderboard to find the rank as if
            // it would have been part of the sorted list
            if (cotd is null || cotd.Leaderboard is null)
            {
                return -1;
            }
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

            return rank;
        }

    }
}
