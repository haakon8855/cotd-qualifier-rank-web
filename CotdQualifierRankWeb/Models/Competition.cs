namespace CotdQualifierRankWeb.Models
{
    public class Competition
    {
        public int CompetitionId { get; set; }
        public int ChallengeId { get; set; }
        public string? MapUid { get; set; }
        public DateTime Date { get; set; }
        public List<Record>? Leaderboard { get; set; }
    }
}
