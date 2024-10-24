using System.Globalization;

namespace CotdQualifierRank.Web.Utils;

public static class Extensions
{
    public static string ToMonthString(this DateTime date)
    {
        return date.ToString("MMMM", CultureInfo.GetCultureInfo("en-UK"));
    }

    public static string ToPageMonthString(this DateTime date)
    {
        return date.ToString("yyyy-MM");
    }

    public static string ToMonthAndYearString(this DateTime date)
    {
        return date.ToString("MMMM yyyy", CultureInfo.GetCultureInfo("en-UK"));
    }

    public static string ToShortDayString(this DateTime date)
    {
        return date.ToString("dd. dddd", CultureInfo.GetCultureInfo("en-UK"));
    }
}
