using CotdQualifierRank.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CotdQualifierRank.Database;

public class CotdContext : DbContext
{
    public CotdContext(DbContextOptions<CotdContext> options)
        : base(options)
    {
    }

    public DbSet<CompetitionEntity> Competitions { get; set; } = default!;
    public DbSet<RecordEntity> Records { get; set; } = default!;
    public DbSet<NadeoCompetitionEntity> NadeoCompetitions { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecordEntity>()
            .HasOne(r => r.Competition)
            .WithMany(c => c.Leaderboard)
            .HasForeignKey("CompetitionId");
        
        // Enable identity insert for NadeoCompetitions
        modelBuilder.Entity<NadeoCompetitionEntity>()
            .Property(p => p.Id)
            .ValueGeneratedNever();
    }
}