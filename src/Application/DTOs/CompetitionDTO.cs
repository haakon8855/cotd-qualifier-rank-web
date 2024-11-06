namespace CotdQualifierRank.Application.DTOs;

public class CompetitionDTO(int competitionId, int challengeId, string? mapUid, DateTime date)
{
    public int CompetitionId { get; } = competitionId;
    public int ChallengeId { get; } = challengeId;
    public string? MapUid { get; } = mapUid;
    public DateTime Date { get; } = date;
}