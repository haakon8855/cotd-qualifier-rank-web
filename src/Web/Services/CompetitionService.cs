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

    public CompetitionDTO? GetCompetitionDTOByCompetitionId(int competitionId, bool includeLeaderboard = true)
    {
        var competition = repository.GetCompetitionByCompetitionId(competitionId, includeLeaderboard);

        if (competition is null)
            return null;
        
        return new CompetitionDTO(
            competition.NadeoCompetitionId,
            competition.NadeoChallengeId,
            competition.NadeoMapUid,
            competition.Date
        );
    }
    
    public Competition? GetCompetitionByCompetitionId(int competitionId, bool includeLeaderboard = true)
    {
        return repository.GetCompetitionByCompetitionId(competitionId, includeLeaderboard);
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

    public List<string> GetMapsUids()
    {
        return repository.GetMapsUids();
    }
}