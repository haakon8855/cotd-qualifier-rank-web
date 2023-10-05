namespace CotdQualifierRankWeb.Models
{
    public class Record
    {
        public int Id { get; set; }
        public int Time { get; set; }

        public override string ToString()
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
    }
}
