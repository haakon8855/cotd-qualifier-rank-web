using CotdQualifierRank.Database.Models;

namespace CotdQualifierRank.Application.DTOs;

public class CompetitionListDTO(Competition[] competitions)
{
    public Competition[] Competitions { get; } = competitions;
}