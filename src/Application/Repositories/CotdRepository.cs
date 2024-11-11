using CotdQualifierRank.Database;
using CotdQualifierRank.Database.Entities;
using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Domain.DomainPrimitives.Nadeo;
using CotdQualifierRank.Application.DTOs;
using CotdQualifierRank.Application.Utils;
using CotdQualifierRank.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CotdQualifierRank.Application.Repositories;

public class CotdRepository(CotdContext context)
{
    public CompetitionModel? GetCompetitionByMapUid(MapUid mapUid, bool includeLeaderboard = true)
    {
        var competitions = context.Competitions.AsNoTracking();
        if (includeLeaderboard)
            competitions = competitions.Include(c => c.Leaderboard);
        var entity = competitions.FirstOrDefault(c => c.NadeoMapUid == mapUid.Value);

        if (entity is null)
            return null;

        return ModelMapper.CompetitionEntityToModel(entity);
    }

    public CompetitionModel? GetCompetitionById(CompetitionId id, bool includeLeaderboard = true)
    {
        var competitions = context.Competitions.AsNoTracking();
        if (includeLeaderboard)
            competitions = competitions.Include(c => c.Leaderboard);
        var entity = competitions.FirstOrDefault(c => c.Id == id.Value);

        if (entity is null)
            return null;

        return ModelMapper.CompetitionEntityToModel(entity);
    }

    public CompetitionModel? GetCompetitionByNadeoCompetitionId(NadeoCompetitionId nadeoCompetitionId,
        bool includeLeaderboard = true)
    {
        var competitions = context.Competitions.AsNoTracking();
        if (includeLeaderboard)
            competitions = competitions.Include(c => c.Leaderboard);
        var entity = competitions.FirstOrDefault(c => c.NadeoCompetitionId == nadeoCompetitionId.Value);

        if (entity is null)
            return null;

        return ModelMapper.CompetitionEntityToModel(entity);
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
            var queriedMonthLastDay =
                new DateTime(year.Value, month.Value, DateTime.DaysInMonth(year.Value, month.Value));
            fetchedComps =
                baseQuery.Where(c => c.Date >= queriedMonthFirstDay && c.Date < queriedMonthLastDay.AddDays(1));
        }

        var competitions = fetchedComps.ToArray();
        return new(competitions.Select(ModelMapper.CompetitionEntityToModel).ToArray());
    }

    public List<RecordModel>? GetLeaderboardByMapUid(MapUid mapUid)
    {
        var leaderboard = context.Competitions
            .AsNoTracking()
            .Include(c => c.Leaderboard)
            .Where(c => c.NadeoMapUid == mapUid.Value)
            .Select(c => c.Leaderboard)
            .FirstOrDefault();

        return leaderboard?.Select(ModelMapper.RecordEntityToModel).ToList();
    }

    public List<RecordModel>? GetLeaderboardByNadeoCompetitionId(NadeoCompetitionId competitionId)
    {
        var leaderboard = context.Competitions
            .AsNoTracking()
            .Include(c => c.Leaderboard)
            .Where(c => c.NadeoCompetitionId == competitionId.Value)
            .Select(c => c.Leaderboard)
            .FirstOrDefault();

        return leaderboard?.Select(ModelMapper.RecordEntityToModel).ToList();
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

    public void AddCompetition(CompetitionModel competition)
    {
        var newCompetition = new CompetitionEntity
        {
            Id = competition.Id,
            NadeoCompetitionId = competition.NadeoCompetitionId,
            NadeoChallengeId = competition.NadeoChallengeId,
            NadeoMapUid = competition.NadeoMapUid,
            Date = competition.Date,
            Leaderboard = competition.Leaderboard?.Select(r => new RecordEntity { Time = r.Time }).ToList(),
            PlayerCount = competition.PlayerCount
        };
        context.Competitions.Add(newCompetition);
        context.SaveChanges();
    }

    public NadeoCompetitionModel? GetNadeoCompetition(DateTime date)
    {
        // Due to inefficient storage here (storing the date as part of the competition name only
        // we have to fetch the whole table and then find the match in memory :/
        var entity = context.NadeoCompetitions
            .ToArray()
            .FirstOrDefault(comp => NadeoCompetitionModel.ParseDate(comp.Name ?? "2020-07-01").Date == date);
        return entity is null ? null : ModelMapper.NadeoCompetitionEntityToModel(entity);
    }

    public void InsertNadeoCompetitions(NadeoCompetitionModel[] nadeoCompetitions)
    {
        var nadeoCompetitionsEntities = nadeoCompetitions
            .Select(ModelMapper.NadeoCompetitionModelToEntity);

        foreach (var comp in nadeoCompetitionsEntities)
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