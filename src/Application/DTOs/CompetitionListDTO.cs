using CotdQualifierRank.Database.Models;

namespace CotdQualifierRank.Application.DTOs;

public class CompetitionListDTO(
    Competition[] competitions,
    int[] playerCounts,
    DateTime oldestDate,
    DateTime newestDate)
{
    public Competition[] Competitions { get; } = competitions;
    public int[] PlayerCounts { get; } = playerCounts;
    public DateTime OldestDate { get; } = oldestDate;
    public DateTime NewestDate { get; } = newestDate;
}