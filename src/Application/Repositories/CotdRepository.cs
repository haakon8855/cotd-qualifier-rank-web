using CotdQualifierRank.Database;
using CotdQualifierRank.Database.Models;
using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Domain.DomainPrimitives.Nadeo;
using CotdQualifierRank.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CotdQualifierRank.Application.Repositories;

public class CotdRepository(CotdContext context)
{
    public Competition? GetCompetitionByMapUid(MapUid mapUid, bool includeLeaderboard = true)
    {
        var competitions = context.Competitions.AsNoTracking();
        if (includeLeaderboard)
            competitions = competitions.Include(c => c.Leaderboard);
        return competitions.FirstOrDefault(c => c.NadeoMapUid == mapUid.Value);
    }
    
    public Competition? GetCompetitionById(CompetitionId id, bool includeLeaderboard = true)
    {
        var competitions = context.Competitions.AsNoTracking();
        if (includeLeaderboard)
            competitions = competitions.Include(c => c.Leaderboard);
        return competitions.FirstOrDefault(c => c.Id == id.Value);
    }

    public Competition? GetCompetitionByNadeoCompetitionId(NadeoCompetitionId nadeoCompetitionId, bool includeLeaderboard = true)
    {
        var competitions = context.Competitions.AsNoTracking();
        if (includeLeaderboard)
            competitions = competitions.Include(c => c.Leaderboard);
        return competitions.FirstOrDefault(c => c.NadeoCompetitionId == nadeoCompetitionId.Value);
    }
    
    public CompetitionListDTO GetCompetitionsAndPlayerCounts(CompetitionYear year, CompetitionMonth month, bool filterAnomalous = false)
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
            fetchedComps = baseQuery.Where(c => c.Date.Year == year.Value && c.Date.Month == month.Value);
        }

        var competitions = fetchedComps.ToArray();
        var competitionPlayerCounts =
            fetchedComps.Select(c => c.Leaderboard == null ? 0 : c.Leaderboard.Count).ToArray();

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
    
    public List<Record>? GetLeaderboardByNadeoCompetitionId(NadeoCompetitionId competitionId)
    {
        return context.Competitions
            .AsNoTracking()
            .Include(c => c.Leaderboard)
            .Where(c => c.NadeoCompetitionId == competitionId.Value)
            .Select(c => c.Leaderboard)
            .FirstOrDefault();
    }
    
    public IEnumerable<MapUid> GetMapsUids()
    {
        return context.Competitions
            .AsNoTracking()
            .Where(c => !string.IsNullOrWhiteSpace(c.NadeoMapUid))
            .OrderBy(c => c.Date)
            .Select(c => c.NadeoMapUid)
            .ToList()
            .Where(MapUid.IsValid)
            .Select(m => new MapUid(m));
    }

    public void AddCompetition(Competition competition)
    {
        context.Competitions.Add(competition);
        context.SaveChanges();
    }
    
    public NadeoCompetition? GetNadeoCompetition(DateTime date)
    {
        // Due to inefficient storage here (storing the date as part of the competition name only
        // we have to fetch the whole table and then find the match in memory :/
        return context.NadeoCompetitions
            .ToArray()
            .FirstOrDefault(comp => NadeoCompetition.ParseDate(comp.Name ?? "2020-07-01").Date == date);
    }

    public void InsertNadeoCompetitions(NadeoCompetition[] nadeoCompetitions)
    {
        foreach (var comp in nadeoCompetitions)
        {
            var compExists = context.NadeoCompetitions.Any(c => c.Id == comp.Id);
            if (!compExists)
            {
                context.NadeoCompetitions.Add(comp);
            }
        }
        context.SaveChanges();
    }
}
