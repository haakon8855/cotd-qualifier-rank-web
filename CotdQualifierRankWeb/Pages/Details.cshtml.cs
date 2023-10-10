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

        public Competition Competition { get; set; } = default!;

        public DetailsModel(Data.CotdContext context, RankController rankController)
        {
            _context = context;
            _rankController = rankController;
        }

        public void Initialise(int? id)
        {
            if (id == null || _context.Competitions == null)
            {
                return;
            }

            var competition = _context.Competitions.Include(c => c.Leaderboard).FirstOrDefault(m => m.Id == id);
            if (competition == null)
            {
                return;
            }
            else
            {
                competition.Leaderboard = competition.Leaderboard.OrderBy(r => r.Time).ToList();
                Competition = competition;
            }

            return;
        }

        public void OnGet(int? id)
        {
            Initialise(id);
        }

        public IActionResult OnPostPB(int? id)
        {
            Initialise(id);
            var response = _rankController.GetAction(Competition.NadeoMapUid, Time).Result;

            // check that response is ok
            if (response is OkObjectResult okObjectResult)
            {
                var content = okObjectResult.Value;
                if (content is GetRankDTO getRankDTO)
                {
                    Rank = getRankDTO.Rank;
                }
            }

            return Page();
        }
    }
}
