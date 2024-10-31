using CotdQualifierRank.Database.Models;

namespace CotdQualifierRank.Web.DTOs;

public class CompetitionListDTO(
    List<Competition> competitions,
    List<int> playerCounts,
    DateTime oldestDate,
    DateTime newestDate)
{
    public List<Competition> Competitions { get; } = competitions;
    public List<int> PlayerCounts { get; } = playerCounts;
    public DateTime OldestDate { get; } = oldestDate;
    public DateTime NewestDate { get; } = newestDate;
}