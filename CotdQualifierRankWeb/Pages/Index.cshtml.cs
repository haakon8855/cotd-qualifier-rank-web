using CotdQualifierRankWeb.Models;
using CotdQualifierRankWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CotdQualifierRankWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CompetitionService _competitionService;

        public List<Competition> PaginatedCompetitions { get; set; } = default!;
        [FromQuery(Name = "pageNo")]
        public int PageNo { get; set; } = 1;
        public int PageCount { get; set; } = 0;
        public readonly int PageSize = 14;

        public IndexModel(CompetitionService competitionService)
        {
            _competitionService = competitionService;
        }

        public List<Competition> Competitions { get; set; } = default!;
        public List<int> CompetitionPlayerCounts { get; set; } = default!;

        public void OnGet()
        {
            var compsAndPlayerCounts = _competitionService.GetCompetitionsAndPlayerCounts(length: PageSize, offset: (PageNo - 1) * PageSize);
            Competitions = compsAndPlayerCounts.Comps;
            CompetitionPlayerCounts = compsAndPlayerCounts.PlayerCounts;

            PageCount = (int)Math.Ceiling((double)compsAndPlayerCounts.TotalComps / (double)PageSize);
            if (PageNo > PageCount)
            {
                PageNo = PageCount;
            }
        }

        public IActionResult OnPostDelete(int id)
        {
            _competitionService.DeleteCompetition(id);
            return RedirectToAction("");
        }
    }
}
