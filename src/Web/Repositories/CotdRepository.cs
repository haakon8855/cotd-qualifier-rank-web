using CotdQualifierRank.Database;
using CotdQualifierRank.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CotdQualifierRank.Web.Repositories;

public class CotdRepository(CotdContext context)
{
    public Competition? GetCompetitionByMapUid(string mapUid)
    {
        return context.Competitions
            .Include(c => c.Leaderboard)
            .FirstOrDefault(c => c.NadeoMapUid == mapUid);
    }
}