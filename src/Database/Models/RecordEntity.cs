namespace CotdQualifierRank.Database.Models;

public class RecordEntity
{
    public int Id { get; set; }
    public int Time { get; set; }
    public CompetitionEntity? Competition { get; set; }

    public string FormattedTime()
    {
        // Return string with time in format "ss.ttt" or "mm:ss.ttt" (if necessary):
        if (Time < 60000)
            return $"{Time / 1000}.{Time % 1000:000}";
        return $"{Time / 60000}:{Time / 1000 % 60:00}.{Time % 1000:000}";
    }

    public string FormattedDiffTime()
    {
        // Return string with time in format "+ss.ttt" or "+mm:ss.ttt" OR "-ss.ttt" or "-mm:ss:ttt" (if negative):
        if (Time < 0)
        {
            if (Time > -60000)
                return $"-{Time / -1000}.{Math.Abs(Time % -1000):000}";
            return $"-{Time / -60000}:{Math.Abs(Time / -1000 % 60):00}.{Math.Abs(Time % -1000):000}";
        }
        return "+" + FormattedTime();
    }

    public static RecordEntity operator -(RecordEntity a, RecordEntity b)
    {
        return new RecordEntity { Id = 0, Time = a.Time - b.Time };
    }
}