using CotdQualifierRankWeb.Data;
using CotdQualifierRankWeb.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CotdQualifierRankWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CotdContext _context;

        public IndexModel(CotdContext context)
        {
            _context = context;
        }

        public List<Competition> Competitions { get; set; } = default!;
        public List<int> CompetitionPlayerCounts { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Competitions != null)
            {
                Competitions = await _context.Competitions.OrderByDescending(c => c.Date).ToListAsync();
                // count the number of players in each comp efficiently for ef core
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                CompetitionPlayerCounts = _context.Competitions.Include(c => c.Leaderboard).Select(c => c.Leaderboard.Count).ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }
    }
}
