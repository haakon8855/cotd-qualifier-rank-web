using System.ComponentModel.DataAnnotations;

namespace CotdQualifierRankWeb.Models
{
    public class Competition
    {
        public static string MapPattern = @"^[A-Za-z0-9_]{26,27}$";

        public int Id { get; set; }

        [Display(Name = "Competition ID")]
        public int NadeoCompetitionId { get; set; }

        [Display(Name = "Challenge ID")]
        public int NadeoChallengeId { get; set; }

        [Display(Name = "Map UID")]

        public string? NadeoMapUid { get; set; }

        public DateTime Date { get; set; }

        public List<Record>? Leaderboard { get; set; }
    }
}
