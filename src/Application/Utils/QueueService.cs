﻿using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;
using CotdQualifierRank.Database.Models;
using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Domain.DomainPrimitives.Nadeo;
using CotdQualifierRank.Application.DTOs;
using CotdQualifierRank.Application.Services;

namespace CotdQualifierRank.Application.Utils;

public class QueueService(
    NadeoApiService nadeoApiService,
    NadeoCompetitionService nadeoCompetitionService,
    CompetitionService competitionService)
{
    private static readonly Queue<MapUid> MapLeaderboardsToFetch = new();
    private static bool IsProcessing { get; set; }

    public static void AddToQueue(MapUid mapUid)
    {
        MapLeaderboardsToFetch.Enqueue(mapUid);
    }

    public static bool QueueContains(MapUid mapUid)
    {
        return MapLeaderboardsToFetch.Contains(mapUid);
    }

    public async Task ProcessMapsAsync()
    {
        if (MapLeaderboardsToFetch.Count == 0)
        {
            IsProcessing = false;
            return;
        }

        if (IsProcessing)
        {
            return;
        }

        IsProcessing = true;
        while (MapLeaderboardsToFetch.Count > 0)
        {
            var mapUid = MapLeaderboardsToFetch.Peek();
            await FetchCompetitionFromNadeo(mapUid);
            MapLeaderboardsToFetch.Dequeue();
        }

        IsProcessing = false;
    }

    private async Task FetchCompetitionFromNadeo(MapUid mapUid)
    {
        var mapResponse = await nadeoApiService.GetTodtInfoForMap(mapUid);
        if (mapResponse is not null)
        {
            var result = await mapResponse.Content.ReadAsStringAsync();
            // get date of totd from response
            var mapTotdInfo = JsonConvert.DeserializeObject<NadeoMapTotdInfoDTO>(result);

            // Response is null or empty or map is not TOTD
            if (mapTotdInfo?.TotdMaps is null || mapTotdInfo.TotdYear == -1)
                return;

            var dayOfWeek = (Array.IndexOf(mapTotdInfo.TotdMaps, mapUid.Value) + 1) % 7;
            var mapTotdDate = ISOWeek.ToDateTime(mapTotdInfo.TotdYear, mapTotdInfo.Week, (DayOfWeek)dayOfWeek);
            mapTotdDate = mapTotdDate.AddHours(19);

            var currentTime =
                TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Central European Standard Time");

            // If the requested map is today's map
            if (currentTime.Date == mapTotdDate.Date)
            {
                // If the time is before 19:30, we assume the leaderboard is not available yet and return null
                if (currentTime < mapTotdDate.AddMinutes(30))
                    return;
            }

            if (mapTotdDate < new DateTime(2020, 11, 16))
                return;

            // Check if we have a NadeoCompetition with that date
            // If the competition name is null, we set the date to 2020-07-01 so that we will never find a match
            var nadeoCompetition = nadeoCompetitionService.GetNadeoCompetition(mapTotdDate.Date);

            if (nadeoCompetition is not null)
            {
                await FetchCompetition(nadeoCompetition, mapUid, mapTotdDate);
            }
            else
            {
                // If not, fetch competitions from Nadeo until we find a match
                nadeoCompetition = await FetchNadeoCompetition(mapTotdDate);
                // When we find it, fetch the leaderboard and store it in the db
                if (nadeoCompetition is null)
                    return;

                await FetchCompetition(nadeoCompetition, mapUid, mapTotdDate);
            }
        }
    }

    private async Task<List<Record>> FetchQualificationLeaderboard(NadeoCompetition nadeoCompetition, NadeoChallengeId challengeId)
    {
        // Fetch the qualification leaderboard
        var fullLeaderboard = new List<Record>();

        for (int i = 0; i < nadeoCompetition.NbPlayers; i += 100)
        {
            var leaderboardFragment = await nadeoApiService.GetLeaderboard(challengeId, 100, i);

            if (leaderboardFragment?.Results != null)
            {
                var records = leaderboardFragment.Results.Select(entry => new Record { Time = entry.Score }).ToArray();
                fullLeaderboard.AddRange(records);
            }
        }

        return fullLeaderboard;
    }

    private async Task FetchCompetition(NadeoCompetition nadeoCompetition, MapUid mapUid, DateTime mapTotdDate)
    {
        var newCompetition = new Competition
        {
            NadeoCompetitionId = nadeoCompetition.Id,
            NadeoChallengeId = await nadeoApiService.GetChallengeId(new NadeoCompetitionId(nadeoCompetition.Id)),
            NadeoMapUid = mapUid.Value,
            Date = mapTotdDate
        };
        newCompetition.Leaderboard =
            await FetchQualificationLeaderboard(nadeoCompetition, new NadeoChallengeId(newCompetition.NadeoChallengeId));
        competitionService.AddCompetition(newCompetition);
    }

    private async Task<NadeoCompetition?> FetchNadeoCompetition(DateTime mapTotdDate)
    {
        var offsetLimit = 10000;
        var offset = 0;
        while (offset < offsetLimit)
        {
            var compResponse = await nadeoApiService.GetCompetitions(100, offset);
            if (compResponse is not null)
            {
                var compResult = await compResponse.Content.ReadAsStringAsync();
                try
                {
                    var competitions = JsonConvert.DeserializeObject<List<NadeoCompetition>>(compResult);
                    if (competitions is null)
                        return null;

                    if (offset == 0)
                        offsetLimit = competitions.First().Id;

                    var cotdCompetitions = competitions.Where(comp => Regex.IsMatch(comp.Name ?? "",
                        @"(COTD|Cup of the Day) 20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]($| #1$)")).ToArray();

                    // store all competitions while searching
                    nadeoCompetitionService.AddNadeoCompetitions(cotdCompetitions);

                    // Check if we have a nadeocompetition with that date
                    // If the competition name is null, we set the date to 2020-07-01 so that we will never find a match
                    var competition = cotdCompetitions
                        .FirstOrDefault(
                            comp => NadeoCompetition.ParseDate(
                                comp.Name ?? "2020-07-01"
                            ).Date == mapTotdDate.Date);

                    // when we find it, fetch the leaderboard and store it in the db
                    if (competition is not null)
                        return competition;
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