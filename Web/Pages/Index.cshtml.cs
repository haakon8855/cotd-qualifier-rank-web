using CotdQualifierRank.Web.Utils;
using CotdQualifierRank.Web.Models;
using CotdQualifierRank.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CotdQualifierRank.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CompetitionService _competitionService;

        public List<Competition> PaginatedCompetitions { get; set; } = default!;

        [FromQuery(Name = "filterAnomalous")]
        public bool FilterAnomalous { get; set; } = false;

        [FromQuery(Name = "pageMonth")]
        public string PageMonth { get; set; } = DateTime.Now.ToString("yyyy-MM");

        public DateTime NewestMonth { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime OldestMonth { get; set; } = new DateTime(2020, 11, 1);

        public IndexModel(CompetitionService competitionService)
        {
            _competitionService = competitionService;
        }

        public List<Competition> Competitions { get; set; } = default!;
        public List<int> CompetitionPlayerCounts { get; set; } = default!;

        public async Task OnGet()
        {
            var year = int.Parse(PageMonth.Split("-")[0]);
            var month = int.Parse(PageMonth.Split("-")[1]);
            var compsAndPlayerCounts = await _competitionService.GetCompetitionsAndPlayerCounts(year, month, filterAnomalous: FilterAnomalous);
            Competitions = compsAndPlayerCounts.Comps;
            CompetitionPlayerCounts = compsAndPlayerCounts.PlayerCounts;
            NewestMonth = new DateTime(compsAndPlayerCounts.NewestDate.Year, compsAndPlayerCounts.NewestDate.Month, 1);
            OldestMonth = new DateTime(compsAndPlayerCounts.OldestDate.Year, compsAndPlayerCounts.OldestDate.Month, 1);
        }

        public string NewPageMonth(int monthsToAdd, bool getMonthName = false)
        {
            DateTime currentMonth = GetPageMonthDateTime();
            DateTime newMonth = currentMonth.AddMonths(monthsToAdd);
            if (!getMonthName)
            {
                return newMonth.ToPageMonthString();
            }
            else
            {
                return newMonth.ToMonthString();
            }
        }

        public DateTime GetPageMonthDateTime()
        {
            return new DateTime(
                year: int.Parse(PageMonth.Split("-")[0]),
                month: int.Parse(PageMonth.Split("-")[1]),
                day: 1
            );
        }
    }
}
