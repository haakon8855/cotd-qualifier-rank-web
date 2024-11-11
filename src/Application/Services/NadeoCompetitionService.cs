using CotdQualifierRank.Domain.Models;
using CotdQualifierRank.Application.Repositories;

namespace CotdQualifierRank.Application.Services;

public class NadeoCompetitionService(CotdRepository repository)
{
    public NadeoCompetitionModel? GetNadeoCompetition(DateTime date)
    {
        return repository.GetNadeoCompetition(date);
    }

    public void AddNadeoCompetitions(NadeoCompetitionModel[] nadeoCompetitions)
    {
        repository.InsertNadeoCompetitions(nadeoCompetitions);
    }
}