using CotdQualifierRankWeb.Data;
using CotdQualifierRankWeb.DTOs;
using CotdQualifierRankWeb.Models;
using CotdQualifierRankWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CotdQualifierRankWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankController : ControllerBase
    {
        CotdContext _context { get; set; }
        NadeoApiController _nadeoApiController { get; set; }
        NadeoCompetitionService _nadeoCompeitionService { get; set; }
        CompetitionService _competitionService { get; set; }

        public RankController(CotdContext context, NadeoApiController nadeoApiController, NadeoCompetitionService nadeoCompetitionService, CompetitionService competitionService)
        {
            _context = context;
            _nadeoApiController = nadeoApiController;
            _nadeoCompeitionService = nadeoCompetitionService;
            _competitionService = competitionService;
        }

        [HttpGet]
        [Route("{mapUID}/{time:int}")]
        public async Task<IActionResult> GetAction(string mapUid, int time)
        {
            var cotd = _context.Competitions.Include(c => c.Leaderboard).FirstOrDefault(c => c.NadeoMapUid == mapUid);

            if (cotd is null)
            {
                cotd = await FetchCompetitionFromNadeo(mapUid);
                if (cotd is null)
                {
                    return NotFound();
                }
            }

            var rank = FindRankInLeaderboard(cotd, time);

            return Ok(new GetRankDTO
            {
                MapUid = mapUid,
                CompetitionId = cotd.NadeoCompetitionId,
                ChallengeId = cotd.NadeoChallengeId,
                Date = cotd.Date,
                Time = time,
                Rank = rank,
            });
            //return Ok(new
            //{
            //    mapUid,
            //    competitionId = cotd.NadeoCompetitionId,
            //    challengeId = cotd.NadeoChallengeId,
            //    date = cotd.Date,
            //    time,
            //    rank,
            //});
        }

        public int FindRankInLeaderboard(Competition cotd, int time)
        {
            // Binary search on the leaderboard to find the rank as if it would have been at the correct location in the list
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

        public async Task<Competition?> FetchCompetitionFromNadeo(string mapUid)
        {
            // get map totd info from nadeo
            var mapResponse = await _nadeoApiController.GetTodtInfoForMap(mapUid);
            if (mapResponse is not null)
            {
                var result = await mapResponse.Content.ReadAsStringAsync();
                // get date of totd from response
                try
                {
                    var mapTotdInfo = JsonConvert.DeserializeObject<NadeoMapTotdInfoDTO>(result);

                    if (mapTotdInfo is null || mapTotdInfo.TotdMaps is null)
                    {
                        return null;
                    }
                    var dayOfWeek = (mapTotdInfo.TotdMaps.IndexOf(mapUid) + 1) % 7;
                    var mapTotdDate = ISOWeek.ToDateTime(mapTotdInfo.TotdYear, mapTotdInfo.Week, (DayOfWeek)dayOfWeek);
                    mapTotdDate = mapTotdDate.AddHours(19);

                    if (mapTotdDate < new DateTime(2020, 11, 2))
                    {
                        return null;
                    }

                    // Check if we have a nadeocompetition with that date
                    // If the competition name is null, we set the date to 2020-07-01 so that we will never find a match
                    var nadeoCompetition = _nadeoCompeitionService
                        .GetNadeoCompetitions()
                        .FirstOrDefault(
                            comp => DateTime.Parse(
                                comp.Name is null ? "2020-07-01" : comp.Name.Split(" ")[1]).Date == mapTotdDate.Date);

                    Competition? cotd = null;
                    if (nadeoCompetition is not null)
                    {
                        cotd = await FetchCompetition(nadeoCompetition, mapUid, mapTotdDate);
                    }
                    else
                    {
                        // If not, fetch competitions from Nadeo until we find a match
                        nadeoCompetition = await FetchNadeoCompetition(mapUid, mapTotdDate);
                        // When we find it, fetch the leaderboard and store it in the db
                        if (nadeoCompetition is null)
                        {
                            return null;
                        }
                        cotd = await FetchCompetition(nadeoCompetition, mapUid, mapTotdDate);
                    }
                    _competitionService.AddCompetition(cotd);
                    return cotd;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
            return null;
        }

        public async Task<List<Record>> FetchQualificationLeaderboard(NadeoCompetition nadeoCompetition, int challengeId)
        {
            // Fetch the qualification leaderboard
            var fullLeaderboard = new List<Record>();

            for (int i = 0; i < nadeoCompetition.NbPlayers; i += 100)
            {
                var leaderboardFragment = await _nadeoApiController.GetLeaderboard(challengeId, 100, i);

                if (leaderboardFragment is not null && leaderboardFragment.Results is not null)
                {
                    var records = leaderboardFragment.Results.Select(entry => new Record { Time = entry.Score }).ToList();
                    fullLeaderboard.AddRange(records);
                }

                // Sleep for 0.5 seconds to not DOS the Nadeo API
                Thread.Sleep(500);
            }

            return fullLeaderboard;
        }

        public async Task<Competition> FetchCompetition(NadeoCompetition nadeoCompetition, string mapUid, DateTime mapTotdDate)
        {
            var newCompetition = new Competition();
            newCompetition.NadeoCompetitionId = nadeoCompetition.Id;
            newCompetition.NadeoChallengeId = await _nadeoApiController.GetChallengeId(nadeoCompetition.Id);
            newCompetition.NadeoMapUid = mapUid;
            newCompetition.Date = mapTotdDate;
            newCompetition.Leaderboard = await FetchQualificationLeaderboard(nadeoCompetition, newCompetition.NadeoChallengeId);
            _competitionService.AddCompetition(newCompetition);

            return newCompetition;
        }

        public async Task<NadeoCompetition?> FetchNadeoCompetition(string mapUid, DateTime mapTotdDate)
        {
            for (int i = 0; i < 100; i++)
            {
                var compResponse = await _nadeoApiController.GetCompetitions(100, i * 100);
                if (compResponse is not null)
                {
                    var compResult = await compResponse.Content.ReadAsStringAsync();
                    try
                    {
                        var competitions = JsonConvert.DeserializeObject<List<NadeoCompetition>>(compResult);
                        if (competitions is null)
                        {
                            return null;
                        }
                        competitions = competitions.Where(comp => Regex.IsMatch(comp.Name is null ? "" : comp.Name, @"COTD 20[0-9][0-9]-[0-9][0-9]-[0-9][0-9] #1$")).ToList();

                        // store all competitions while searching
                        _nadeoCompeitionService.AddNadeoCompetitions(competitions);

                        // Check if we have a nadeocompetition with that date
                        // If the competition name is null, we set the date to 2020-07-01 so that we will never find a match
                        var competition = competitions
                                .FirstOrDefault(
                                    comp => DateTime.Parse(
                                        comp.Name is null ? "2020-07-01" : comp.Name.Split(" ")[1]
                                        ).Date == mapTotdDate.Date);

                        // when we find it, fetch the leaderboard and store it in the db
                        return competition;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return null;
                    }
                }
                // Sleep for 0.5 seconds to not DOS the Nadeo API
                Thread.Sleep(500);
            }
            return null;
        }
    }
}
