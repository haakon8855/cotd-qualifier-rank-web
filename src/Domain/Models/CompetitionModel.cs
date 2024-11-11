using CotdQualifierRank.Domain.DomainPrimitives;

namespace CotdQualifierRank.Domain.Models;

public class CompetitionModel(
    int id,
    int nadeoCompetitionId,
    int nadeoChallengeId,
    string nadeoMapUid,
    DateTime date,
    List<Time> leaderboard,
    int playerCount)
{
    public int Id { get; } = id;
    public int NadeoCompetitionId { get; } = nadeoCompetitionId;
    public int NadeoChallengeId { get; } = nadeoChallengeId;
    public string NadeoMapUid { get; } = nadeoMapUid;
    public DateTime Date { get; } = date;
    public List<Time>? Leaderboard { get; } = leaderboard;
    public int PlayerCount { get; } = playerCount;
}