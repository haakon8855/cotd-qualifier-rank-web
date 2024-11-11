﻿using CotdQualifierRank.Application.DTOs;
using CotdQualifierRank.Domain.Models;
using CotdQualifierRank.Database.Entities;
using CotdQualifierRank.Domain.DomainPrimitives;

namespace CotdQualifierRank.Application.Utils;

public static class ModelMapper
{
    public static CompetitionModel CompetitionEntityToModel(CompetitionEntity entity)
    {
        return new CompetitionModel(
            entity.Id,
            entity.NadeoCompetitionId,
            entity.NadeoChallengeId,
            entity.NadeoMapUid,
            entity.Date,
            entity.Leaderboard is not null ? entity.Leaderboard.Select(RecordEntityToTimeDomainPrimitive).ToList() : [],
            entity.PlayerCount
        );
    }

    public static Time RecordEntityToTimeDomainPrimitive(RecordEntity entity)
    {
        return new Time(entity.Time);
    }

    public static NadeoCompetitionModel NadeoCompetitionEntityToModel(NadeoCompetitionEntity entity)
    {
        return new NadeoCompetitionModel(
            entity.Id,
            entity.liveId ?? string.Empty,
            entity.Name ?? string.Empty,
            entity.Description ?? string.Empty,
            entity.NbPlayers
        );
    }

    public static NadeoCompetitionModel NadeoCompetitionDTOToModel(NadeoCompetitionDTO dto)
    {
        return new NadeoCompetitionModel(
            dto.Id,
            dto.liveId ?? string.Empty,
            dto.Name ?? string.Empty,
            dto.Description ?? string.Empty,
            dto.NbPlayers
        );
    }

    public static NadeoCompetitionEntity NadeoCompetitionModelToEntity(NadeoCompetitionModel model)
    {
        return new NadeoCompetitionEntity
        {
            Id = model.Id,
            liveId = model.LiveId,
            Name = model.Name,
            Description = model.Description,
            NbPlayers = model.NbPlayers
        };
    }
}