namespace CotdQualifierRank.Application.DTOs;

public class RankDTO(
    string mapUid,
    int competitionId,
    int challengeId,
    DateTime date,
    int time,
    int rank,
    int playerCount,
    bool leaderboardIsEmpty)
{
    public string MapUid { get; set; } = mapUid;
    public int CompetitionId { get; set; } = competitionId;
    public int ChallengeId { get; set; } = challengeId;
    public DateTime Date { get; set; } = date;
    public int Time { get; set; } = time;
    public int Rank { get; set; } = rank;
    public int PlayerCount { get; set; } = playerCount;
    public bool LeaderboardIsEmpty { get; set; } = leaderboardIsEmpty;
}
