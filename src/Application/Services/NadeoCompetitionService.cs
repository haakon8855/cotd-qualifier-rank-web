using CotdQualifierRank.Database.Entities;
using CotdQualifierRank.Application.Repositories;

namespace CotdQualifierRank.Application.Services;

public class NadeoCompetitionService(CotdRepository repository)
{
    public NadeoCompetitionEntity? GetNadeoCompetition(DateTime date)
    {
        return repository.GetNadeoCompetition(date);
    }

    public void AddNadeoCompetitions(NadeoCompetitionEntity[] nadeoCompetitions)
    {
        repository.InsertNadeoCompetitions(nadeoCompetitions);
    }
}