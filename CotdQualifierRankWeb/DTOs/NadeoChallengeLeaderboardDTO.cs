namespace CotdQualifierRankWeb.DTOs
{
    public class NadeoChallengeLeaderboardDTO
    {
        public int ChallengeId { get; set; }
        public int Cardinal { get; set; }
        public List<NadeoChallengeLeaderboardEntryDTO>? Results { get; set; }
    }
}
