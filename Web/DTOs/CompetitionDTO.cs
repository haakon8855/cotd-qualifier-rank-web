namespace CotdQualifierRank.Web.DTOs;

public class CompetitionDTO
{
    public int CompetitionId { get; set; } = 0;
    public int ChallengeId { get; set; } = 0;
    public string? MapUid { get; set; } = default!;
    public DateTime Date { get; set; } = default!;
}
