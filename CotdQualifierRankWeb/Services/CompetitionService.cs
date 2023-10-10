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

        public (List<Competition> Comps, List<int> PlayerCounts) GetCompetitionsAndPlayerCounts()
        {
            var baseQuery = _context.Competitions.OrderByDescending(c => c.Date);
            var competitions = baseQuery.ToList();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var competitionPlayerCounts = baseQuery.Select(c => c.Leaderboard.Count).ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            return (Comps: competitions, PlayerCounts: competitionPlayerCounts);
        }
    }
}
