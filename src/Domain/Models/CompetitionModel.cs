using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Domain.DomainPrimitives.Nadeo;

namespace CotdQualifierRank.Domain.Models;

public class CompetitionModel(
    CompetitionId id,
    NadeoCompetitionId nadeoCompetitionId,
    NadeoChallengeId nadeoChallengeId,
    MapUid nadeoMapUid,
    DateTime date,
    List<Time> leaderboard,
    int playerCount)
{
    public CompetitionId Id { get; } = id;
    public NadeoCompetitionId NadeoCompetitionId { get; } = nadeoCompetitionId;
    public NadeoChallengeId NadeoChallengeId { get; } = nadeoChallengeId;
    public MapUid NadeoMapUid { get; } = nadeoMapUid;
    public DateTime Date { get; } = date;
    public List<Time>? Leaderboard { get; } = leaderboard;
    public int PlayerCount { get; } = playerCount;
}