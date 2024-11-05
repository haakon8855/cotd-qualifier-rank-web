using CotdQualifierRank.Database.Models;
using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Web.Utils;
using CotdQualifierRank.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CotdQualifierRank.Web.Pages;

public class IndexModel(CompetitionService competitionService) : PageModel
{
    [FromQuery(Name = "filterAnomalous")] public bool FilterAnomalous { get; set; } = false;

    [FromQuery(Name = "pageMonth")] public string PageMonth { get; set; } = DateTime.Now.ToString("yyyy-MM");

    public DateTime NewestMonth { get; set; } = new(DateTime.Now.Year, DateTime.Now.Month, 1);
    public DateTime OldestMonth { get; set; } = new(2020, 11, 1);

    public Competition[] Competitions { get; set; } = default!;
    public int[] CompetitionPlayerCounts { get; set; } = default!;

    public IActionResult OnGet()
    {
        var year = int.Parse(PageMonth.Split("-")[0]);
        var month = int.Parse(PageMonth.Split("-")[1]);
        
        if (!CompetitionYear.IsValid(year) || !CompetitionMonth.IsValid(month))
            return RedirectToPage();
        
        var compsAndPlayerCounts =
            competitionService.GetCompetitionListDTO(new CompetitionYear(year), new CompetitionMonth(month),
                filterAnomalous: FilterAnomalous);
        Competitions = compsAndPlayerCounts.Competitions;
        CompetitionPlayerCounts = compsAndPlayerCounts.PlayerCounts;
        NewestMonth = new DateTime(compsAndPlayerCounts.NewestDate.Year, compsAndPlayerCounts.NewestDate.Month, 1);
        OldestMonth = new DateTime(compsAndPlayerCounts.OldestDate.Year, compsAndPlayerCounts.OldestDate.Month, 1);
        
        return Page();
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