using CotdQualifierRankWeb.Data;
using CotdQualifierRankWeb.Models;

namespace CotdQualifierRankWeb.Services
{
    public class NadeoCompetitionService
    {
        public CotdContext _context { get; set; } = default!;

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
    }
}
