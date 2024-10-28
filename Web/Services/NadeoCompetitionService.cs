using CotdQualifierRank.Database;
using CotdQualifierRank.Database.Models;

namespace CotdQualifierRank.Web.Services;

public class NadeoCompetitionService(CotdContext context)
{
    public List<NadeoCompetition> GetNadeoCompetitions()
    {
        return context.NadeoCompetitions.ToList();
    }

    public NadeoCompetition? GetNadeoCompetition(int id)
    {
        return context.NadeoCompetitions.Find(id);
    }

    public void AddNadeoCompetitions(List<NadeoCompetition> nadeoCompetitions)
    {
        foreach (var comp in nadeoCompetitions)
        {
            var compExists = context.NadeoCompetitions.Any(c => c.Id == comp.Id);
            if (!compExists)
            {
                context.NadeoCompetitions.Add(comp);
            }
        }

        context.SaveChanges();
    }

    public bool DateExists(DateTime date)
    {
        return context.NadeoCompetitions
            .Where(c => c.Name != null)
            .ToList()
            .Select(c => NadeoCompetition.ParseDate(c.Name is null ? "2020-07-01" : c.Name).Date)
            .Any(d => d == date);
    }
}