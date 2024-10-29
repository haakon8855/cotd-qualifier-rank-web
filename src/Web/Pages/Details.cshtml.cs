using CotdQualifierRank.Database;
using CotdQualifierRank.Web.Controllers;
using CotdQualifierRank.Web.DTOs;
using CotdQualifierRank.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CotdQualifierRank.Web.Pages;

public class DetailsModel : PageModel
{
    private readonly CotdContext _context;

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
    public List<Record> FirstSeedDifference { get; set; } = default!;
    public Dictionary<string, string> PageStatistics { get; set; } = new Dictionary<string, string>();

    public DetailsModel(CotdContext context, RankController rankController)
    {
        _context = context;
        _rankController = rankController;
    }

    private async Task Initialise(int? id)
    {
        if (PageNo < 1)
        {
            PageNo = 1;
        }
        if (id is null || _context.Competitions is null)
        {
            return;
        }

        var competition = await _context.Competitions.Include(c => c.Leaderboard).FirstOrDefaultAsync(m => m.Id == id);
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

        // Calculate first seed difference
        var firstSeed = Competition.Leaderboard[0];
        FirstSeedDifference = PaginatedLeaderboard.Select(r => r - firstSeed).ToList();
    }

    public async Task OnGet(int? id)
    {
        await Initialise(id);

        if (TempData.TryGetValue("Rank", out var rankData) && rankData is not null)
        {
            Rank = (int)rankData;
        }
        if (TempData.TryGetValue("Time", out var timeData) && timeData is not null)
        {
            Time = (int)timeData;
        }
    }

    public async Task<IActionResult> OnPostPB(int? id)
    {
        await Initialise(id);
        if (Competition.NadeoMapUid is null)
        {
            return RedirectToPage();
        }
        var response = _rankController.GetQualifierRank(Competition.NadeoMapUid, Time);

        if (response is OkObjectResult okObjectResult)
        {
            var content = okObjectResult.Value;
            if (content is RankDTO rankDTO)
            {
                TempData["Rank"] = rankDTO.Rank;
                TempData["Time"] = Time;
            }
        }
        return RedirectToPage(new { id = Competition.Id });
    }
}
