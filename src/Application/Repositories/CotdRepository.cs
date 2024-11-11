using CotdQualifierRank.Database;
using CotdQualifierRank.Database.Entities;
using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Domain.DomainPrimitives.Nadeo;
using CotdQualifierRank.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CotdQualifierRank.Application.Repositories;

public class CotdRepository(CotdContext context)
{
    public CompetitionEntity? GetCompetitionByMapUid(MapUid mapUid, bool includeLeaderboard = true)
    {
        var competitions = context.Competitions.AsNoTracking();
        if (includeLeaderboard)
            competitions = competitions.Include(c => c.Leaderboard);
        return competitions.FirstOrDefault(c => c.NadeoMapUid == mapUid.Value);
    }

    public CompetitionEntity? GetCompetitionById(CompetitionId id, bool includeLeaderboard = true)
    {
        var competitions = context.Competitions.AsNoTracking();
        if (includeLeaderboard)
            competitions = competitions.Include(c => c.Leaderboard);
        return competitions.FirstOrDefault(c => c.Id == id.Value);
    }

    public CompetitionEntity? GetCompetitionByNadeoCompetitionId(NadeoCompetitionId nadeoCompetitionId,
        bool includeLeaderboard = true)
    {
        var competitions = context.Competitions.AsNoTracking();
        if (includeLeaderboard)
            competitions = competitions.Include(c => c.Leaderboard);
        return competitions.FirstOrDefault(c => c.NadeoCompetitionId == nadeoCompetitionId.Value);
    }

    public CompetitionListDTO GetCompetitionsAndPlayerCounts(CompetitionYear year, CompetitionMonth month,
        bool filterAnomalous = false)
    {
        var baseQuery = context.Competitions
            .OrderByDescending(c => c.Date)
            .AsNoTracking();

        IQueryable<CompetitionEntity> fetchedComps;
        if (filterAnomalous)
        {
            fetchedComps = context.NadeoCompetitions
                .Join(
                    context.Competitions,
                    nc => nc.Id,
                    c => c.NadeoCompetitionId,
                    (nc, c) => new { NadeoCompetition = nc, Competition = c })
                .AsNoTracking()
                .Where(
                    jc => jc.Competition.Leaderboard == null ||
                          jc.Competition.PlayerCount == 0 ||
                          jc.NadeoCompetition.NbPlayers != jc.Competition.PlayerCount)
                .Select(jc => jc.Competition)
                .OrderByDescending(c => c.Date);
        }
        else
        {
            var queriedMonthFirstDay = new DateTime(year.Value, month.Value, 1);
            var queriedMonthLastDay = new DateTime(year.Value, month.Value, DateTime.DaysInMonth(year.Value, month.Value));
            fetchedComps = baseQuery.Where(c => c.Date >= queriedMonthFirstDay && c.Date < queriedMonthLastDay.AddDays(1));
        }

        var competitions = fetchedComps.ToArray();

        return new(competitions);
    }

    public List<RecordEntity>? GetLeaderboardByMapUid(MapUid mapUid)
    {
        return context.Competitions
            .AsNoTracking()
            .Include(c => c.Leaderboard)
            .Where(c => c.NadeoMapUid == mapUid.Value)
            .Select(c => c.Leaderboard)
            .FirstOrDefault();
    }

    public List<RecordEntity>? GetLeaderboardByNadeoCompetitionId(NadeoCompetitionId competitionId)
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

    public void AddCompetition(CompetitionEntity competition)
    {
        context.Competitions.Add(competition);
        context.SaveChanges();
    }

    public NadeoCompetitionEntity? GetNadeoCompetition(DateTime date)
    {
        // Due to inefficient storage here (storing the date as part of the competition name only
        // we have to fetch the whole table and then find the match in memory :/
        return context.NadeoCompetitions
            .ToArray()
            .FirstOrDefault(comp => NadeoCompetitionEntity.ParseDate(comp.Name ?? "2020-07-01").Date == date);
    }

    public void InsertNadeoCompetitions(NadeoCompetitionEntity[] nadeoCompetitions)
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