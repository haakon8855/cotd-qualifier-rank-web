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

        public Competition? GetCompetitionByMapUid(string mapUid, bool includeLeaderboard = true)
        {
            if (includeLeaderboard)
            {
                return _context.Competitions
                    .Include(c => c.Leaderboard)
                    .FirstOrDefault(c => c.NadeoMapUid == mapUid);
            }
            return _context.Competitions
                .FirstOrDefault(c => c.NadeoMapUid == mapUid);
        }

        public Competition? GetCompetition(int competitionId)
        {
            return _context.Competitions
                .FirstOrDefault(c => c.NadeoCompetitionId == competitionId);
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

        public (
            List<Competition> Comps,
            List<int> PlayerCounts,
            DateTime OldestDate,
            DateTime NewestDate
        )
            GetCompetitionsAndPlayerCounts(
            int year,
            int month,
            bool filterAnomalous = false
        )
        {
            var baseQuery = _context.Competitions.OrderByDescending(c => c.Date);

            var oldestDate = baseQuery.LastOrDefault()?.Date ?? new DateTime(2020, 11, 02);
            var newestDate = baseQuery.FirstOrDefault()?.Date ?? DateTime.Now;

            IQueryable<Competition> fetchedComps = baseQuery;
            if (filterAnomalous)
            {
                baseQuery = _context.NadeoCompetitions.Join(_context.Competitions.Include(c => c.Leaderboard), nc => nc.Id, c => c.NadeoCompetitionId, (nc, c) => new { NadeoCompetition = nc, Competition = c })
                    .Where(jc => jc.Competition.Leaderboard == null || jc.Competition.Leaderboard.Count == 0 || jc.NadeoCompetition.NbPlayers != jc.Competition.Leaderboard.Count)
                    .Select(jc => jc.Competition).OrderByDescending(c => c.Date);
                fetchedComps = baseQuery;
            }
            else
            {
                fetchedComps = baseQuery.Where(c => c.Date.Year == year && c.Date.Month == month);
            }
            var competitions = fetchedComps.ToList();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var competitionPlayerCounts = fetchedComps.Select(c => c.Leaderboard.Count).ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return (
                Comps: competitions,
                PlayerCounts: competitionPlayerCounts,
                OldestDate: oldestDate,
                NewestDate: newestDate
            );
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

        public async Task<List<string?>> GetMapsUids()
        {
            return await _context.Competitions
                .Where(c => c.NadeoMapUid != null)
                .OrderBy(c => c.Date)
                .Select(c => c.NadeoMapUid)
                .ToListAsync();
        }
    }
}
