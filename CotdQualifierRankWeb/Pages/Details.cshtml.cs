using CotdQualifierRankWeb.Controllers;
using CotdQualifierRankWeb.DTOs;
using CotdQualifierRankWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CotdQualifierRankWeb.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly Data.CotdContext _context;

        private readonly RankController _rankController;

        [BindProperty]
        public int Time { get; set; } = 0;

        public int Rank { get; set; } = 0;

        [FromQuery(Name = "pageNo")]
        public int PageNo { get; set; } = 1;
        public readonly int PageSize = 64;
        public int PageCount { get; set; } = 0;
        public int PlayerCount { get; set; } = 0;

        public Competition Competition { get; set; } = default!;
        public List<Record> PaginatedLeaderboard { get; set; } = default!;
        public Dictionary<string, string> PageStatistics { get; set; } = new Dictionary<string, string>();

        public DetailsModel(Data.CotdContext context, RankController rankController)
        {
            _context = context;
            _rankController = rankController;
        }

        private void Initialise(int? id)
        {
            if (PageNo < 1)
            {
                PageNo = 1;
            }
            if (id is null || _context.Competitions is null)
            {
                return;
            }

            var competition = _context.Competitions.Include(c => c.Leaderboard).FirstOrDefault(m => m.Id == id);
            if (competition is null || competition.Leaderboard is null)
            {
                return;
            }
            else
            {
                PlayerCount = competition.Leaderboard.Count;
                PageCount = (int)Math.Ceiling((double)PlayerCount / (double)PageSize);
                if (PageNo > PageCount)
                {
                    PageNo = PageCount;
                }
                competition.Leaderboard = competition.Leaderboard.OrderBy(r => r.Time).ToList();
                PaginatedLeaderboard = competition.Leaderboard.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();
                Competition = competition;
            }

            CalculateStatistics();

            return;
        }

        private void CalculateStatistics()
        {
            if (Competition is null || Competition.Leaderboard is null || PaginatedLeaderboard is null)
            {
                return;
            }

            // Calculate cutoff time of the division above
            Record betterDivCutoff;
            if (PageNo == 1)
            {
                betterDivCutoff = Competition.Leaderboard[0];
            }
            else
            {
                betterDivCutoff = Competition.Leaderboard[(PageNo - 1) * PageSize - 1];
            }

            // Calculate cutoff of current division
            Record currentDivCutoff = PaginatedLeaderboard.Last();

            // Calculate window of current division
            Record currentDivWindow = currentDivCutoff - betterDivCutoff;

            PageStatistics.Add("BetterDivCutoff", betterDivCutoff.FormattedTime());
            PageStatistics.Add("CurrentDivCutoff", currentDivCutoff.FormattedTime());
            PageStatistics.Add("CurrentDivWindow", currentDivWindow.FormattedTime());
        }

        public void OnGet(int? id)
        {
            Initialise(id);
        }

        public IActionResult OnPostPB(int? id)
        {
            Initialise(id);
            if (Competition.NadeoMapUid is null)
            {
                return Page();
            }
            var response = _rankController.GetAction(Competition.NadeoMapUid, Time).Result;

            if (response is OkObjectResult okObjectResult)
            {
                var content = okObjectResult.Value;
                if (content is GetRankDTO getRankDTO)
                {
                    Console.WriteLine(getRankDTO.Rank);
                    Rank = getRankDTO.Rank;
                }
            }

            return Page();
        }
    }
}
