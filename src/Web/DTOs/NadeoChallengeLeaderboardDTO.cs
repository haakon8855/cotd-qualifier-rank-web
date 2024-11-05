namespace CotdQualifierRank.Web.DTOs;

public class NadeoChallengeLeaderboardDTO
{
    public int ChallengeId { get; set; }
    public int Cardinal { get; set; }
    public NadeoChallengeLeaderboardEntryDTO[]? Results { get; set; }
}
