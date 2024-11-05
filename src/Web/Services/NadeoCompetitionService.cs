using CotdQualifierRank.Database.Models;
using CotdQualifierRank.Web.Repositories;

namespace CotdQualifierRank.Web.Services;

public class NadeoCompetitionService(CotdRepository repository)
{
    public NadeoCompetition? GetNadeoCompetition(DateTime date)
    {
        return repository.GetNadeoCompetition(date);
    }

    public void AddNadeoCompetitions(NadeoCompetition[] nadeoCompetitions)
    {
        repository.InsertNadeoCompetitions(nadeoCompetitions);
    }
}