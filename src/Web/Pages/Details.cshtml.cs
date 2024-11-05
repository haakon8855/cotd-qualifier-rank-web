using CotdQualifierRank.Database.Models;
using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CotdQualifierRank.Web.Pages;

public class DetailsModel(RankService rankService, CompetitionService competitionService) : PageModel
{
    [BindProperty] public int SearchBoxTime { get; set; }

    public int Rank { get; set; }

    [FromQuery(Name = "pageNo")] public int PageNo { get; set; } = 1;

    public const int PageSize = 64;
    public int PageCount { get; set; }
    public int PlayerCount { get; set; }

    public Competition Competition { get; set; } = default!;
    public List<Record> PaginatedLeaderboard { get; set; } = default!;
    public List<Record> FirstSeedDifference { get; set; } = default!;
    public Dictionary<string, string> PageStatistics { get; set; } = new();

    private void Initialise(int? id)
    {
        if (PageNo < 1)
            PageNo = 1;

        if (id is null)
            return;

        var competition = competitionService.GetCompetitionById(id ?? 0);
        if (competition?.Leaderboard is null)
            return;

        PlayerCount = competition.Leaderboard.Count;
        PageCount = (int)Math.Ceiling((double)PlayerCount / PageSize);
        if (PageNo > PageCount)
            PageNo = PageCount;

        competition.Leaderboard = competition.Leaderboard.OrderBy(r => r.Time).ToList();
        PaginatedLeaderboard = competition.Leaderboard.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();
        Competition = competition;

        CalculateStatistics();
    }

    private void CalculateStatistics()
    {
        if (Competition is null ||
            Competition.Leaderboard is null ||
            PaginatedLeaderboard is null ||
            Competition.Leaderboard.Count == 0)
        {
            return;
        }

        // Calculate cutoff time of the division above
        Record betterDivCutoff;
        if (PageNo == 1)
            betterDivCutoff = Competition.Leaderboard[0];
        else
            betterDivCutoff = Competition.Leaderboard[(PageNo - 1) * PageSize - 1];

        // Calculate cutoff of current division
        Record currentDivCutoff = PaginatedLeaderboard.Last();

        // Calculate window of current division
        Record currentDivWindow = currentDivCutoff - betterDivCutoff;

        PageStatistics.Add("BetterDivCutoff", betterDivCutoff.FormattedTime());
        PageStatistics.Add("CurrentDivCutoff", currentDivCutoff.FormattedTime());
        PageStatistics.Add("CurrentDivWindow", currentDivWindow.FormattedTime());

        // Calculate first seed difference
        var firstSeed = Competition.Leaderboard[0];
        FirstSeedDifference = PaginatedLeaderboard.Select(r => r - firstSeed).ToList();
    }

    public void OnGet(int? id)
    {
        Initialise(id);

        if (TempData.TryGetValue("Rank", out var rankData) && rankData is not null)
        {
            Rank = (int)rankData;
        }

        if (TempData.TryGetValue("Time", out var timeData) && timeData is not null)
        {
            SearchBoxTime = (int)timeData;
        }
    }

    public IActionResult OnPostPB(int? id)
    {
        Initialise(id);

        if (!Time.IsValid(SearchBoxTime))
            return RedirectToPage(new { id = Competition.Id });
        
        var rankDTO = rankService.GetRank(Competition, new Time(SearchBoxTime));

        if (rankDTO is null)
            return RedirectToPage(new { id = Competition.Id });

        TempData["Rank"] = rankDTO.Rank;
        TempData["Time"] = SearchBoxTime;

        return RedirectToPage(new { id = Competition.Id });
    }
}