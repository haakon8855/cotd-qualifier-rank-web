using CotdQualifierRank.Database.Models;

namespace CotdQualifierRank.Application.DTOs;

public class CompetitionListDTO(CompetitionEntity[] competitions)
{
    public CompetitionEntity[] Competitions { get; } = competitions;
}