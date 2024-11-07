using CotdQualifierRank.Database.Models;
using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Application.Utils;
using CotdQualifierRank.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CotdQualifierRank.Web.Pages;

public class IndexModel(CompetitionService competitionService) : PageModel
{
    [FromQuery(Name = "filterAnomalous")] public bool FilterAnomalous { get; set; } = false;

    [FromQuery(Name = "pageMonth")] public string PageMonth { get; set; } = DateTime.Now.ToString("yyyy-MM");

    public readonly DateTime OldestMonth = new(2020, 11, 1);
    public readonly DateTime NewestMonth = new(DateTime.Now.Year, DateTime.Now.Month, 1);

    public Competition[] Competitions { get; private set; } = default!;

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
        
        return Page();
    }

    public string NewPageMonth(int monthsToAdd, bool getMonthName = false)
    {
        var currentMonth = GetPageMonthDateTime();
        var newMonth = currentMonth.AddMonths(monthsToAdd);
        if (!getMonthName)
            return newMonth.ToPageMonthString();
        return newMonth.ToMonthString();
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