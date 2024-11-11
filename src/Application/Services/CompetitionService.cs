﻿using CotdQualifierRank.Database.Entities;
using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Domain.DomainPrimitives.Nadeo;
using CotdQualifierRank.Application.DTOs;
using CotdQualifierRank.Application.Repositories;

namespace CotdQualifierRank.Application.Services;

public class CompetitionService(CotdRepository repository)
{
    public CompetitionDTO? GetCompetitionDTO(MapUid mapUid, bool includeLeaderboard = true)
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

    public CompetitionDTO? GetCompetitionDTO(NadeoCompetitionId competitionId, bool includeLeaderboard = true)
    {
        var competition = repository.GetCompetitionByNadeoCompetitionId(competitionId, includeLeaderboard);

        if (competition is null)
            return null;
        
        return new CompetitionDTO(
            competition.NadeoCompetitionId,
            competition.NadeoChallengeId,
            competition.NadeoMapUid,
            competition.Date
        );
    }
    
    public CompetitionEntity? GetCompetition(CompetitionId id, bool includeLeaderboard = true)
    {
        return repository.GetCompetitionById(id, includeLeaderboard);
    }

    public List<int> GetLeaderboard(MapUid mapUid)
    {
        var leaderboard = repository.GetLeaderboardByMapUid(mapUid);

        if (leaderboard is null)
            return [];

        return leaderboard
            .Select(r => r.Time)
            .Order()
            .ToList();
    }
    
    public List<int> GetLeaderboard(NadeoCompetitionId competitionId)
    {
        var leaderboard = repository.GetLeaderboardByNadeoCompetitionId(competitionId);

        if (leaderboard is null)
            return [];

        return leaderboard
            .Select(r => r.Time)
            .Order()
            .ToList();
    }

    public void AddCompetition(CompetitionEntity? competition)
    {
        if (competition is not null)
            repository.AddCompetition(competition);
    }

    public CompetitionListDTO GetCompetitionListDTO(CompetitionYear year, CompetitionMonth month, bool filterAnomalous = false)
    {
        return repository.GetCompetitionsAndPlayerCounts(year, month, filterAnomalous);
    }

    public string[] GetMapsUids()
    {
        return repository.GetMapsUids().Select(m => m.Value).ToArray();
    }
}