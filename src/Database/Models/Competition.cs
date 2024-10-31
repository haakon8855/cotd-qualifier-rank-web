using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CotdQualifierRank.Database.Models;

public class Competition
{
    public int Id { get; set; }

    [Display(Name = "Competition ID")]
    public int NadeoCompetitionId { get; set; }

    [Display(Name = "Challenge ID")]
    public int NadeoChallengeId { get; set; }

    [Display(Name = "Map UID")]
    public string? NadeoMapUid { get; set; }

    public DateTime Date { get; set; }

    public List<Record>? Leaderboard { get; set; }

    public static string MapPattern = @"^[A-Za-z0-9_]{26,27}$";
    
    public static bool IsValidMapUid(string mapUid)
    {
        return Regex.IsMatch(mapUid, MapPattern);
    }
}
