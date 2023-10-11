namespace CotdQualifierRankWeb.Models
{
    public class Record
    {
        public int Id { get; set; }
        public int Time { get; set; }

        public string FormattedTime()
        {
            // Return string with time in format "ss.ttt" or "mm:ss.ttt" (if necessary):
            if (Time < 60000)
            {
                return $"{Time / 1000}.{Time % 1000:000}";
            }
            else
            {
                return $"{Time / 60000}:{Time / 1000 % 60:00}.{Time % 1000:000}";
            }
        }

        public string FormattedDiffTime()
        {
            // Return string with time in format "+ss.ttt" or "+mm:ss.ttt" OR "-ss.ttt" or "-mm:ss:ttt" (if negative):
            if (Time < 0)
            {
                if (Time > -60000)
                {
                    return $"-{Time / -1000}.{Math.Abs(Time % -1000):000}";
                }
                else
                {
                    return $"-{Time / -60000}:{Math.Abs(Time / -1000 % 60):00}.{Math.Abs(Time % -1000):000}";
                }
            }
            else
            {
                return "+" + FormattedTime();
            }
        }

        public static Record operator -(Record a, Record b)
        {
            return new Record { Id = 0, Time = a.Time - b.Time };
        }
    }
}
