using CotdQualifierRankWeb.Models;

namespace CotdQualifierRankWeb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(CotdContext context)
        {
            if (context.Competitions.Any())
            {
                return; // DB has been seeded
            }

            int numChallenges = 10;
            var competitions = new Competition[numChallenges];
            for (int i = 0; i < numChallenges; i++)
            {
                // random world record time between 31 and 70 seconds:
                var worldRecord = 31000 + (int)((70000 - 31000) * new Random().NextDouble());
                var leaderboard = GenerateRandomLeaderboard(100, worldRecord, worldRecord + 20000);
                competitions[i] = new Competition
                {
                    Date = DateTime.Today.AddDays(-i).AddHours(19),
                    NadeoChallengeId = 8888 - 30 * i,
                    NadeoCompetitionId = 4444 - 20 * i,
                    // Generate random alphabetic string of length 27:
                    NadeoMapUid = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", 27)
                                           .Select(s => s[new Random().Next(s.Length)]).ToArray()),
                    Leaderboard = leaderboard.ToList()
                };
            }

            context.Competitions.AddRange(competitions);
            context.SaveChanges();
        }

        public static Record[] GenerateRandomLeaderboard(int numRecords, int min, int max)
        {
            var records = new Record[numRecords];
            for (int i = 0; i < numRecords; i++)
            {
                records[i] = new Record { Time = min + (int)((max - min) * new System.Random().NextDouble()) };
            }

            return records;
        }
    }
}
