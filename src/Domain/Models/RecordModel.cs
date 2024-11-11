namespace CotdQualifierRank.Domain.Models;

public class RecordModel
{
    public RecordModel(int id, int time)
    {
        Id = id;
        Time = time;
    }

    public int Id { get; }
    public int Time { get; }

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

    public static RecordModel operator -(RecordModel a, RecordModel b)
    {
        return new RecordModel(0, a.Time - b.Time);
    }
}