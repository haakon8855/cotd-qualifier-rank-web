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

        public async Task<Competition?> GetCompetitionByMapUid(string mapUid, bool includeLeaderboard = true)
        {
            if (includeLeaderboard)
            {
                return await _context.Competitions
                    .Include(c => c.Leaderboard)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.NadeoMapUid == mapUid);
            }
            return await _context.Competitions
                .FirstOrDefaultAsync(c => c.NadeoMapUid == mapUid);
        }

        public async Task<Competition?> GetCompetition(int competitionId, bool includeLeaderboard = true)
        {
            if (includeLeaderboard)
            {
                return await _context.Competitions
                    .Include(c => c.Leaderboard)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.NadeoCompetitionId == competitionId);
            }
            return await _context.Competitions
                .FirstOrDefaultAsync(c => c.NadeoCompetitionId == competitionId);
        }

        public async Task AddCompetition(Competition? competition)
        {
            if (competition is not null)
            {
                _context.Competitions.Add(competition);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddLeaderboardToCompetition(int id, List<Record> leaderboard)
        {
            var competition = _context.Competitions.Include(c => c.Leaderboard).FirstOrDefault(c => c.Id == id);
            if (competition is not null)
            {
                competition.Leaderboard = leaderboard;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(
            List<Competition> Comps,
            List<int> PlayerCounts,
            DateTime OldestDate,
            DateTime NewestDate
        )>
            GetCompetitionsAndPlayerCounts(
            int year,
            int month,
            bool filterAnomalous = false
        )
        {
            var baseQuery = _context.Competitions
                .OrderByDescending(c => c.Date)
                .AsNoTracking();

            var oldestDate = baseQuery.LastOrDefault()?.Date ?? new DateTime(2020, 11, 02);
            var newestDate = baseQuery.FirstOrDefault()?.Date ?? DateTime.Now;

            IQueryable<Competition> fetchedComps = baseQuery;
            if (filterAnomalous)
            {
                baseQuery = _context.NadeoCompetitions.Join(_context.Competitions.Include(c => c.Leaderboard), nc => nc.Id, c => c.NadeoCompetitionId, (nc, c) => new { NadeoCompetition = nc, Competition = c })
                    .AsNoTracking()
                    .Where(
                        jc => jc.Competition.Leaderboard == null ||
                        jc.Competition.Leaderboard.Count == 0 ||
                        jc.NadeoCompetition.NbPlayers != jc.Competition.Leaderboard.Count)
                    .Select(jc => jc.Competition)
                    .OrderByDescending(c => c.Date);
                fetchedComps = baseQuery;
            }
            else
            {
                fetchedComps = baseQuery.Where(c => c.Date.Year == year && c.Date.Month == month);
            }
            var competitions = await fetchedComps.ToListAsync();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var competitionPlayerCounts = await fetchedComps.Select(c => c.Leaderboard.Count).ToListAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return (
                Comps: competitions,
                PlayerCounts: competitionPlayerCounts,
                OldestDate: oldestDate,
                NewestDate: newestDate
            );
        }

        public async Task DeleteCompetition(int id)
        {
            var competition = _context.Competitions.Include(c => c.Leaderboard).FirstOrDefault(c => c.Id == id);
            if (competition is not null)
            {
                _context.Competitions.Remove(competition);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<string?>> GetMapsUids()
        {
            return await _context.Competitions
                .AsNoTracking()
                .Where(c => c.NadeoMapUid != null)
                .OrderBy(c => c.Date)
                .Select(c => c.NadeoMapUid)
                .ToListAsync();
        }
    }
}
