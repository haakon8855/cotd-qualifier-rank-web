﻿using CotdQualifierRank.Database;
using CotdQualifierRank.Database.Models;
using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Web.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CotdQualifierRank.Web.Repositories;

public class CotdRepository(CotdContext context)
{
    public Competition? GetCompetitionByMapUid(MapUid mapUid, bool includeLeaderboard = true)
    {
        var competitions = context.Competitions.AsNoTracking();
        if (includeLeaderboard)
            competitions = competitions.Include(c => c.Leaderboard);
        return competitions.FirstOrDefault(c => c.NadeoMapUid == mapUid.Value);
    }

    public Competition? GetCompetitionByCompetitionId(int competitionId, bool includeLeaderboard = true)
    {
        var competitions = context.Competitions.AsNoTracking();
        if (includeLeaderboard)
            competitions = competitions.Include(c => c.Leaderboard);
        return competitions.FirstOrDefault(c => c.NadeoCompetitionId == competitionId);
    }
    
    public CompetitionListDTO GetCompetitionsAndPlayerCounts(int year, int month, bool filterAnomalous = false)
    {
        var baseQuery = context.Competitions
            .OrderByDescending(c => c.Date)
            .AsNoTracking();

        var oldestDate = baseQuery.LastOrDefault()?.Date ?? new DateTime(2020, 11, 02);
        var newestDate = baseQuery.FirstOrDefault()?.Date ?? DateTime.Now;

        IQueryable<Competition> fetchedComps;
        if (filterAnomalous)
        {
            fetchedComps = context.NadeoCompetitions
                .Join(
                    context.Competitions.Include(c => c.Leaderboard),
                    nc => nc.Id,
                    c => c.NadeoCompetitionId,
                    (nc, c) => new { NadeoCompetition = nc, Competition = c })
                .AsNoTracking()
                .Where(
                    jc => jc.Competition.Leaderboard == null ||
                          jc.Competition.Leaderboard.Count == 0 ||
                          jc.NadeoCompetition.NbPlayers != jc.Competition.Leaderboard.Count)
                .Select(jc => jc.Competition)
                .OrderByDescending(c => c.Date);
        }
        else
        {
            fetchedComps = baseQuery.Where(c => c.Date.Year == year && c.Date.Month == month);
        }

        var competitions = fetchedComps.ToList();
        var competitionPlayerCounts =
            fetchedComps.Select(c => c.Leaderboard == null ? 0 : c.Leaderboard.Count).ToList();

        return new(
            competitions,
            competitionPlayerCounts,
            oldestDate,
            newestDate
        );
    }

    public List<Record>? GetLeaderboardByMapUid(MapUid mapUid)
    {
        return context.Competitions
            .AsNoTracking()
            .Include(c => c.Leaderboard)
            .Where(c => c.NadeoMapUid == mapUid.Value)
            .Select(c => c.Leaderboard)
            .FirstOrDefault();
    }
    
    public List<Record>? GetLeaderboardByCompetitionId(int competitionId)
    {
        return context.Competitions
            .AsNoTracking()
            .Include(c => c.Leaderboard)
            .Where(c => c.NadeoCompetitionId == competitionId)
            .Select(c => c.Leaderboard)
            .FirstOrDefault();
    }
    
    public List<string?> GetMapsUids()
    {
        return context.Competitions
            .AsNoTracking()
            .Where(c => c.NadeoMapUid != null)
            .OrderBy(c => c.Date)
            .Select(c => c.NadeoMapUid)
            .ToList();
    }

    public void AddCompetition(Competition competition)
    {
        context.Competitions.Add(competition);
        context.SaveChangesAsync();
    }
}
