using CotdQualifierRank.Database.Models;
using CotdQualifierRank.Web.Repositories;

namespace CotdQualifierRank.Web.Services;

public class NadeoCompetitionService(CotdRepository repository)
{
    public List<NadeoCompetition> GetAllNadeoCompetitions()
    {
        return repository.GetAllNadeoCompetitions();
    }

    public void AddNadeoCompetitions(List<NadeoCompetition> nadeoCompetitions)
    {
        repository.InsertNadeoCompetitions(nadeoCompetitions);
    }
}