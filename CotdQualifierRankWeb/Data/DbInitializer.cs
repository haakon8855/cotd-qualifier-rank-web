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
                var leaderboard = GenerateRandomLeaderboard(100, 30000, 50000);
                competitions[i] = new Competition
                {
                    Date = DateTime.Today.AddDays(-i),
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
