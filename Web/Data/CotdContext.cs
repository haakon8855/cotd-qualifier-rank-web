using CotdQualifierRank.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CotdQualifierRank.Web.Data;

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
        // enable identity insert for NadeoCompetitions
        modelBuilder.Entity<NadeoCompetition>()
            .Property(p => p.Id)
            .ValueGeneratedNever();
    }
}

