namespace CotdQualifierRank.Database.Entities;

public class CompetitionEntity
{
    public int Id { get; set; }

    public int NadeoCompetitionId { get; set; }

    public int NadeoChallengeId { get; set; }

    public required string NadeoMapUid { get; set; }

    public DateTime Date { get; set; }

    public List<RecordEntity>? Leaderboard { get; set; }
    
    public int PlayerCount { get; set; }
}
