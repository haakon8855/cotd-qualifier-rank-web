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
            if (competition is not null)
            {
                _context.Competitions.Add(competition);
                _context.SaveChanges();
            }
        }

        public void AddLeaderboardToCompetition(int id, List<Record> leaderboard)
        {
            var competition = _context.Competitions.Include(c => c.Leaderboard).FirstOrDefault(c => c.Id == id);
            if (competition is not null)
            {
                competition.Leaderboard = leaderboard;
                _context.SaveChanges();
            }
        }

        public (List<Competition> Comps, List<int> PlayerCounts, int TotalComps) GetCompetitionsAndPlayerCounts(int length = 0, int offset = 0, bool filterSuspicious = false)
        {
            if (length < 1)
            {
                length = int.MaxValue;
            }

            var baseQuery = _context.Competitions.OrderByDescending(c => c.Date);
            if (filterSuspicious)
            {
                baseQuery = _context.NadeoCompetitions.Join(_context.Competitions.Include(c => c.Leaderboard), nc => nc.Id, c => c.NadeoCompetitionId, (nc, c) => new { NadeoCompetition = nc, Competition = c })
                    .Where(jc => jc.Competition.Leaderboard == null || jc.Competition.Leaderboard.Count == 0 || jc.NadeoCompetition.NbPlayers != jc.Competition.Leaderboard.Count)
                    .Select(jc => jc.Competition).OrderByDescending(c => c.Date);
            }

            var totalComps = baseQuery.Count();
            var fetchedComps = baseQuery.Skip(offset).Take(length);
            var competitions = fetchedComps.ToList();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var competitionPlayerCounts = fetchedComps.Select(c => c.Leaderboard.Count).ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            return (Comps: competitions, PlayerCounts: competitionPlayerCounts, TotalComps: totalComps);
        }

        public void DeleteCompetition(int id)
        {
            var competition = _context.Competitions.Include(c => c.Leaderboard).FirstOrDefault(c => c.Id == id);
            if (competition is not null)
            {
                _context.Competitions.Remove(competition);
                _context.SaveChanges();
            }
        }
    }
}
