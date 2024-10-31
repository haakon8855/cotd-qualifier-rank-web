using CotdQualifierRank.Database.Models;
using CotdQualifierRank.Web.DTOs;
using CotdQualifierRank.Web.Repositories;
using CotdQualifierRank.Web.Utils;

namespace CotdQualifierRank.Web.Services;

public class RankService(CotdRepository repository)
{
    public RankDTO? GetRank(string mapUid, int time)
    {
        var cotd = repository.GetCompetitionByMapUid(mapUid);

        if (cotd is null)
        {
            if (!QueueService.QueueContains(mapUid))
            {
                QueueService.AddToQueue(mapUid);
            }

            return null;
        }

        return GetRank(cotd, time);
    }
    
    public RankDTO? GetRank(Competition cotd, int time)
    {
        var rank = FindRankInLeaderboard(cotd, time);

        return new RankDTO
        {
            MapUid = cotd.NadeoMapUid ?? "",
            CompetitionId = cotd.NadeoCompetitionId,
            ChallengeId = cotd.NadeoChallengeId,
            Date = cotd.Date,
            Time = time,
            Rank = rank,
            PlayerCount = cotd.Leaderboard?.Count ?? 0,
            LeaderboardIsEmpty = cotd.Leaderboard is null || cotd.Leaderboard.Count == 0,
        };
    }
    
    private static int FindRankInLeaderboard(Competition? cotd, int time)
    {
        // Binary search on the leaderboard to find the rank as if
        // it would have been part of the sorted list
        if (cotd is null || cotd.Leaderboard is null)
        {
            return -1;
        }

        cotd.Leaderboard.Sort((a, b) => a.Time.CompareTo(b.Time));
        int rank = 0;
        int min = 0;
        int max = cotd.Leaderboard.Count;
        while (min < max)
        {
            int mid = (min + max) / 2;
            if (cotd.Leaderboard[mid].Time < time)
            {
                min = mid + 1;
            }
            else
            {
                max = mid;
            }
        }

        rank = min + 1;

        return rank;
    }
}