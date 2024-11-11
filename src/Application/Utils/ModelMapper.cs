using CotdQualifierRank.Application.DTOs;
using CotdQualifierRank.Domain.Models;
using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Domain.DomainPrimitives.Nadeo;
using CotdQualifierRank.Database.Entities;

namespace CotdQualifierRank.Application.Utils;

public static class ModelMapper
{
    public static CompetitionModel CompetitionEntityToModel(CompetitionEntity entity)
    {
        return new CompetitionModel(
            new CompetitionId(entity.Id),
            new NadeoCompetitionId(entity.NadeoCompetitionId),
            new NadeoChallengeId(entity.NadeoChallengeId),
            new MapUid(entity.NadeoMapUid),
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
            new NadeoCompetitionId(entity.Id),
            entity.LiveId ?? string.Empty,
            entity.Name ?? string.Empty,
            entity.Description ?? string.Empty,
            entity.NbPlayers
        );
    }

    public static NadeoCompetitionModel NadeoCompetitionDTOToModel(NadeoCompetitionDTO dto)
    {
        return new NadeoCompetitionModel(
            new NadeoCompetitionId(dto.Id),
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
            Id = model.Id.Value,
            LiveId = model.LiveId,
            Name = model.Name,
            Description = model.Description,
            NbPlayers = model.NbPlayers
        };
    }
}