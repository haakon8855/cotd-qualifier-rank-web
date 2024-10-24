using System.Text.RegularExpressions;

namespace CotdQualifierRank.Web.Models;

public class NadeoCompetition
{
    public int Id { get; set; }
    public string? liveId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int NbPlayers { get; set; }

    public static DateTime ParseDate(string input)
    {
        string[] dateFormats = {
            "COTD yyyy-MM-dd #1",
            "Cup of the Day yyyy-MM-dd #1",
            "Cup of the Day yyyy-MM-dd"
        };
        string[] nameFormats = {
            @"COTD 20[0-9][0-9]-[0-9][0-9]-[0-9][0-9] #1$",
            @"Cup of the Day 20[0-9][0-9]-[0-9][0-9]-[0-9][0-9] #1$",
            @"Cup of the Day 20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]$",
        };
        string nameFormat = @"20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]";

        for (int i = 0; i < dateFormats.Length; i++)
        {
            Match match = Regex.Match(input, nameFormat);
            if (match.Success)
            {
                if (DateTime.TryParse(match.Value, out DateTime date))
                {
                    return date;
                }
            }
        }

        return DateTime.Parse("2020-07-01");
    }
}
