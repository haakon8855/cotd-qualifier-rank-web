namespace CotdQualifierRank.Domain.Models;

public class CompetitionModel
{
    public CompetitionModel(
        int id,
        int nadeoCompetitionId,
        int nadeoChallengeId,
        string nadeoMapUid,
        DateTime date,
        List<RecordModel> leaderboard,
        int playerCount)
    {
        Id = id;
        NadeoCompetitionId = nadeoCompetitionId;
        NadeoChallengeId = nadeoChallengeId;
        NadeoMapUid = nadeoMapUid;
        Date = date;
        Leaderboard = leaderboard;
        PlayerCount = playerCount;
    }

    public int Id { get; set; }
    public int NadeoCompetitionId { get; set; }
    public int NadeoChallengeId { get; set; }
    public string NadeoMapUid { get; set; }
    public DateTime Date { get; set; }
    public List<RecordModel>? Leaderboard { get; set; }
    public int PlayerCount { get; set; }
}