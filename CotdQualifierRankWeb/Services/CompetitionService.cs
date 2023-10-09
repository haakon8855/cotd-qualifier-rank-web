using CotdQualifierRankWeb.Data;
using CotdQualifierRankWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CotdQualifierRankWeb.Services
{
    public class CompetitionService
    {
        public CotdContext _context { get; set; } = default!;

        public CompetitionService(CotdContext context)
        {
            _context = context;
        }

        public Competition? GetCompetitionByMapUid(string mapUid)
        {
            return _context.Competitions.Include(c => c.Leaderboard).FirstOrDefault(c => c.NadeoMapUid == mapUid);
        }

        public void AddCompetition(Competition? competition)
        {
            if (competition != null)
            {
                _context.Competitions.Add(competition);
                _context.SaveChanges();
            }
        }

        public void AddLeaderboardToCompetition(int id, List<Record> leaderboard)
        {
            var competition = _context.Competitions.Include(c => c.Leaderboard).FirstOrDefault(c => c.Id == id);
            if (competition != null)
            {
                competition.Leaderboard = leaderboard;
                _context.SaveChanges();
            }
        }
    }
}
