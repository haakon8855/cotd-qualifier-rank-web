namespace CotdQualifierRank.Web.DTOs;

public class CompetitionDTO
{
    public int CompetitionId { get; set; }
    public int ChallengeId { get; set; }
    public string? MapUid { get; set; }
    public DateTime Date { get; set; }
}