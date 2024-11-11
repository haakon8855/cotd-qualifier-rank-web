using CotdQualifierRank.Database.Models;
using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Application.DTOs;
using CotdQualifierRank.Application.Repositories;
using CotdQualifierRank.Application.Utils;

namespace CotdQualifierRank.Application.Services;

public class RankService(CotdRepository repository)
{
    public RankDTO? GetRank(MapUid mapUid, Time time)
    {
        var cotd = repository.GetCompetitionByMapUid(mapUid);

        if (cotd is null)
        {
            if (!QueueService.QueueContains(mapUid))
                QueueService.AddToQueue(mapUid);
            return null;
        }

        return GetRank(cotd, time);
    }

    public RankDTO? GetRank(CompetitionEntity cotd, Time time)
    {
        var rank = FindRankInLeaderboard(cotd, time);

        return new RankDTO(
            cotd.NadeoMapUid,
            cotd.NadeoCompetitionId,
            cotd.NadeoChallengeId,
            cotd.Date,
            time.Value,
            rank,
            cotd.Leaderboard?.Count ?? 0,
            cotd.Leaderboard is null || cotd.Leaderboard.Count == 0
        );
    }

    private static int FindRankInLeaderboard(CompetitionEntity? cotd, Time time)
    {
        // Binary search on the leaderboard to find the rank as if
        // it would have been part of the sorted list
        if (cotd?.Leaderboard is null)
            return -1;

        cotd.Leaderboard.Sort((a, b) => a.Time.CompareTo(b.Time));
        var rank = 0;
        var min = 0;
        var max = cotd.Leaderboard.Count;
        while (min < max)
        {
            var mid = (min + max) / 2;
            if (cotd.Leaderboard[mid].Time < time.Value)
                min = mid + 1;
            else
                max = mid;
        }
        rank = min + 1;
        return rank;
    }
}