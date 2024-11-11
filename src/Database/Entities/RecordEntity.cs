namespace CotdQualifierRank.Database.Entities;

public class RecordEntity
{
    public int Id { get; set; }
    public int Time { get; set; }
    public CompetitionEntity? Competition { get; set; }

}