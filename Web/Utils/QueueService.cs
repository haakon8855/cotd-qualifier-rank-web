using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;
using CotdQualifierRank.Web.Controllers;
using CotdQualifierRank.Web.DTOs;
using CotdQualifierRank.Web.Models;
using CotdQualifierRank.Web.Services;

namespace CotdQualifierRank.Web.Utils
{
    public class QueueService
    {
        private static readonly Queue<string> MapLeaderboardsToFetch = new Queue<string>();
        public static bool IsProcessing { get; private set; } = false;

        NadeoApiController _nadeoApiController { get; set; }
        NadeoCompetitionService _nadeoCompeitionService { get; set; }
        CompetitionService _competitionService { get; set; }

        public QueueService(NadeoApiController nadeoApiController, NadeoCompetitionService nadeoCompetitionService, CompetitionService competitionService)
        {
            _nadeoApiController = nadeoApiController;
            _nadeoCompeitionService = nadeoCompetitionService;
            _competitionService = competitionService;
        }

        public static void AddToQueue(string mapUid)
        {
            QueueService.MapLeaderboardsToFetch.Enqueue(mapUid);
        }

        public static bool QueueContains(string mapUid)
        {
            return QueueService.MapLeaderboardsToFetch.Contains(mapUid);
        }

        public async Task ProcessMapsAsync()
        {
            if (QueueService.MapLeaderboardsToFetch.Count == 0)
            {
                IsProcessing = false;
                return;
            }
            if (IsProcessing)
            {
                return;
            }

            IsProcessing = true;
            while (QueueService.MapLeaderboardsToFetch.Count > 0)
            {
                var mapUid = QueueService.MapLeaderboardsToFetch.Peek();
                await FetchCompetitionFromNadeo(mapUid);
                QueueService.MapLeaderboardsToFetch.Dequeue();
            }
            IsProcessing = false;
        }

        public async Task<Competition?> FetchCompetitionFromNadeo(string mapUid)
        {
            // check if mapUid matches pattern
            if (!Competition.IsValidMapUid(mapUid))
            {
                return null;
            }
            var mapResponse = await _nadeoApiController.GetTodtInfoForMap(mapUid);
            if (mapResponse is not null)
            {
                var result = await mapResponse.Content.ReadAsStringAsync();
                // get date of totd from response
                try
                {
                    var mapTotdInfo = JsonConvert.DeserializeObject<NadeoMapTotdInfoDTO>(result);

                    // Response is null or empty or map is not TOTD
                    if (mapTotdInfo is null ||
                        mapTotdInfo.TotdMaps is null ||
                        mapTotdInfo.TotdYear == -1)
                    {
                        return null;
                    }

                    var dayOfWeek = (mapTotdInfo.TotdMaps.IndexOf(mapUid) + 1) % 7;
                    var mapTotdDate = ISOWeek.ToDateTime(mapTotdInfo.TotdYear, mapTotdInfo.Week, (DayOfWeek)dayOfWeek);
                    mapTotdDate = mapTotdDate.AddHours(19);

                    var currentTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Central European Standard Time");

                    // If the requested map is today's map
                    if (currentTime.Date == mapTotdDate.Date)
                    {
                        // If the time is before 19:30, we assume the leaderboard is not available yet and return null
                        if (currentTime < mapTotdDate.AddMinutes(30))
                        {
                            return null;
                        }
                    }

                    if (mapTotdDate < new DateTime(2020, 11, 16))
                    {
                        return null;
                    }

                    // Check if we have a nadeocompetition with that date
                    // If the competition name is null, we set the date to 2020-07-01 so that we will never find a match
                    var nadeoCompetition = _nadeoCompeitionService
                        .GetNadeoCompetitions()
                        .FirstOrDefault(
                            comp => NadeoCompetition.ParseDate(
                                comp.Name is null ? "2020-07-01" : comp.Name
                                ).Date == mapTotdDate.Date);

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
            await _competitionService.AddCompetition(newCompetition);

            return newCompetition;
        }

        public async Task<NadeoCompetition?> FetchNadeoCompetition(string mapUid, DateTime mapTotdDate)
        {
            var offsetLimit = 10000;
            int offset = 0;
            while (offset < offsetLimit)
            {
                var compResponse = await _nadeoApiController.GetCompetitions(100, offset);
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
                        if (offset == 0)
                        {
                            offsetLimit = competitions.First().Id;
                        }


                        //var cotdCompetitions = competitions.Where(comp => Regex.IsMatch(comp.Name is null ? "" : comp.Name, @"COTD 20[0-9][0-9]-[0-9][0-9]-[0-9][0-9] #1$")).ToList();
                        var cotdCompetitions = competitions.Where(comp => Regex.IsMatch(comp.Name is null ? "" : comp.Name, @"(COTD|Cup of the Day) 20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]($| #1$)")).ToList();

                        // store all competitions while searching
                        _nadeoCompeitionService.AddNadeoCompetitions(cotdCompetitions);

                        // Check if we have a nadeocompetition with that date
                        // If the competition name is null, we set the date to 2020-07-01 so that we will never find a match
                        var competition = cotdCompetitions
                                .FirstOrDefault(
                                    comp => NadeoCompetition.ParseDate(
                                        comp.Name is null ? "2020-07-01" : comp.Name
                                        ).Date == mapTotdDate.Date);

                        // when we find it, fetch the leaderboard and store it in the db
                        if (competition is not null)
                        {
                            return competition;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return null;
                    }
                }
                offset += 100;
            }
            return null;
        }
    }
}
