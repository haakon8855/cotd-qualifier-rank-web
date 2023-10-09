using CotdQualifierRankWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CotdQualifierRankWeb.Data
{
    public class CotdContext : DbContext
    {
        public CotdContext(DbContextOptions<CotdContext> options)
            : base(options)
        {
        }

        public DbSet<Competition> Competitions { get; set; } = default!;
        public DbSet<Record> Records { get; set; } = default!;
        public DbSet<NadeoCompetition> NadeoCompetitions { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Competition>().ToTable("Competition");
            modelBuilder.Entity<Record>().ToTable("Record");
        }
    }
}
