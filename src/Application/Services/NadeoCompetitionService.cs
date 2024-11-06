using CotdQualifierRank.Database.Models;
using CotdQualifierRank.Application.Repositories;

namespace CotdQualifierRank.Application.Services;

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