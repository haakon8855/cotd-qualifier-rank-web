using CotdQualifierRank.Web.Data;
using CotdQualifierRank.Web.Models;

namespace CotdQualifierRank.Web.Services
{
    public class NadeoCompetitionService
    {
        private readonly CotdContext _context;

        public NadeoCompetitionService(CotdContext context)
        {
            _context = context;
        }

        public List<NadeoCompetition> GetNadeoCompetitions()
        {
            return _context.NadeoCompetitions.ToList();
        }

        public NadeoCompetition? GetNadeoCompetition(int id)
        {
            return _context.NadeoCompetitions.Find(id);
        }

        public void AddNadeoCompetitions(List<NadeoCompetition> nadeoCompetitions)
        {
            foreach (var comp in nadeoCompetitions)
            {
                var compExists = _context.NadeoCompetitions.Any(c => c.Id == comp.Id);
                if (!compExists)
                {
                    _context.NadeoCompetitions.Add(comp);
                }
            }
            _context.SaveChanges();
        }

        public bool DateExists(DateTime date)
        {
            return _context.NadeoCompetitions
                .Where(c => c.Name != null)
                .ToList()
                .Select(c => NadeoCompetition.ParseDate(c.Name is null ? "2020-07-01" : c.Name).Date)
                .Any(d => d == date);
        }
    }
}
