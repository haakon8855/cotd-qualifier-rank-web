using CotdQualifierRank.Domain.Models;

namespace CotdQualifierRank.Application.DTOs;

public class CompetitionListDTO(CompetitionModel[] competitions)
{
    public CompetitionModel[] Competitions { get; } = competitions;
}