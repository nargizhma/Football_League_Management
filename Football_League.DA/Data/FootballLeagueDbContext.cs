using Microsoft.EntityFrameworkCore;
using Football_League.DAL.Entities;

namespace Football_League.DAL.Data
{
    public class FootballLeagueDbContext : DbContext
    {
        public FootballLeagueDbContext(DbContextOptions<FootballLeagueDbContext> options)
            : base(options)
        {
        }

        public DbSet<Stadium> Stadiums { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchGoalscorer> MatchGoalscorers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Stadium configuration
            modelBuilder.Entity<Stadium>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Capacity).IsRequired();
            });

            // Team configuration
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired();
                entity.Property(e => e.StadiumId).IsRequired();

                // Unique constraints
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.Code).IsUnique();

                // Relationships
                entity.HasOne(e => e.Stadium)
                    .WithMany(s => s.Teams)
                    .HasForeignKey(e => e.StadiumId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Players)
                    .WithOne(p => p.Team)
                    .HasForeignKey(p => p.TeamId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.HomeMatches)
                    .WithOne(m => m.HomeTeam)
                    .HasForeignKey(m => m.HomeTeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.AwayMatches)
                    .WithOne(m => m.AwayTeam)
                    .HasForeignKey(m => m.AwayTeamId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Player configuration
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.JerseyNumber).IsRequired();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TeamId).IsRequired();
                entity.Property(e => e.GoalsScored).IsRequired().HasDefaultValue(0);

                // Unique constraint: jersey number per team
                entity.HasIndex(e => new { e.TeamId, e.JerseyNumber }).IsUnique();

                // Relationship
                entity.HasOne(e => e.Team)
                    .WithMany(t => t.Players)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.MatchGoalscorers)
                    .WithOne(mg => mg.Player)
                    .HasForeignKey(mg => mg.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Match configuration
            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.HomeTeamId).IsRequired();
                entity.Property(e => e.AwayTeamId).IsRequired();
                entity.Property(e => e.WeekNumber).IsRequired();
                entity.Property(e => e.HomeGoals).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.AwayGoals).IsRequired().HasDefaultValue(0);

                // Relationships
                entity.HasOne(e => e.HomeTeam)
                    .WithMany(t => t.HomeMatches)
                    .HasForeignKey(e => e.HomeTeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AwayTeam)
                    .WithMany(t => t.AwayMatches)
                    .HasForeignKey(e => e.AwayTeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.MatchGoalscorers)
                    .WithOne(mg => mg.Match)
                    .HasForeignKey(mg => mg.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MatchGoalscorer configuration
            modelBuilder.Entity<MatchGoalscorer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MatchId).IsRequired();
                entity.Property(e => e.PlayerId).IsRequired();
                entity.Property(e => e.GoalsCount).IsRequired();

                // Relationships
                entity.HasOne(e => e.Match)
                    .WithMany(m => m.MatchGoalscorers)
                    .HasForeignKey(e => e.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Player)
                    .WithMany(p => p.MatchGoalscorers)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
