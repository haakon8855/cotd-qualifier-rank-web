namespace CotdQualifierRank.Web.DTOs;

public class RankDTO
{
    public string MapUid { get; set; } = "";
    public int CompetitionId { get; set; }
    public int ChallengeId { get; set; } 
    public DateTime Date { get; set; } 
    public int Time { get; set; } 
    public int Rank { get; set; }
    public int PlayerCount { get; set; }
    public bool LeaderboardIsEmpty { get; set; }
}
