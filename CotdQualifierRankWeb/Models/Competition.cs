namespace CotdQualifierRankWeb.Models
{
    public class Competition
    {
        int CompetitionId { get; set; }
        int ChallengeId { get; set; }
        string? MapUid { get; set; }
        DateTime Date { get; set; }
        List<Record>? Leaderboard { get; set; }
    }
}
