using CotdQualifierRankWeb.Models;
using CotdQualifierRankWeb.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CotdQualifierRankWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CompetitionService _competitionService;

        public IndexModel(CompetitionService competitionService)
        {
            _competitionService = competitionService;
        }

        public List<Competition> Competitions { get; set; } = default!;
        public List<int> CompetitionPlayerCounts { get; set; } = default!;

        public void OnGet()
        {
            var compsAndPlayerCounts = _competitionService.GetCompetitionsAndPlayerCounts();
            Competitions = compsAndPlayerCounts.Comps;
            CompetitionPlayerCounts = compsAndPlayerCounts.PlayerCounts;
        }

        public void OnPostDelete(int id)
        {
            _competitionService.DeleteCompetition(id);
            OnGet();
        }
    }
}
