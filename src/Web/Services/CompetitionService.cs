using CotdQualifierRank.Database.Models;
using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Web.DTOs;
using CotdQualifierRank.Web.Repositories;

namespace CotdQualifierRank.Web.Services;

public class CompetitionService(CotdRepository repository)
{
    public CompetitionDTO? GetCompetitionDTOByMapUid(MapUid mapUid, bool includeLeaderboard = true)
    {
        var competition = repository.GetCompetitionByMapUid(mapUid, includeLeaderboard);
        
        if (competition is null)
            return null;
        
        return new CompetitionDTO(
            competition.NadeoCompetitionId,
            competition.NadeoChallengeId,
            competition.NadeoMapUid,
            competition.Date
        );
    }

    public CompetitionDTO? GetCompetitionDTOById(int competitionId, bool includeLeaderboard = true)
    {
        var competition = repository.GetCompetitionByNadeoId(competitionId, includeLeaderboard);

        if (competition is null)
            return null;
        
        return new CompetitionDTO(
            competition.NadeoCompetitionId,
            competition.NadeoChallengeId,
            competition.NadeoMapUid,
            competition.Date
        );
    }
    
    public Competition? GetCompetitionById(int id, bool includeLeaderboard = true)
    {
        return repository.GetCompetitionById(id, includeLeaderboard);
    }
    
    public Competition? GetCompetitionByNadeoId(int nadeoCompetitionId, bool includeLeaderboard = true)
    {
        return repository.GetCompetitionByNadeoId(nadeoCompetitionId, includeLeaderboard);
    }

    public List<int> GetLeaderboardByMapUid(MapUid mapUid)
    {
        var leaderboard = repository.GetLeaderboardByMapUid(mapUid);

        if (leaderboard is null)
            return [];

        return leaderboard
            .Select(r => r.Time)
            .Order()
            .ToList();
    }
    
    public List<int> GetLeaderboardByCompetitionId(int competitionId)
    {
        var leaderboard = repository.GetLeaderboardByCompetitionId(competitionId);

        if (leaderboard is null)
            return [];

        return leaderboard
            .Select(r => r.Time)
            .Order()
            .ToList();
    }

    public void AddCompetition(Competition? competition)
    {
        if (competition is not null)
            repository.AddCompetition(competition);
    }

    public CompetitionListDTO GetCompetitionsAndPlayerCounts(int year, int month, bool filterAnomalous = false)
    {
        return repository.GetCompetitionsAndPlayerCounts(year, month, filterAnomalous);
    }

    public IEnumerable<string> GetMapsUids()
    {
        return repository.GetMapsUids().Select(m => m.Value);
    }
}