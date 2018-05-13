using Microsoft.EntityFrameworkCore;
using TarokScoreBoard.Core.Entities;

namespace TarokScoreBoard.Infrastructure
{
  public partial class TarokDbContext : DbContext
    {
        public TarokDbContext(DbContextOptions<TarokDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Game> Game { get; set; }
        public virtual DbSet<GamePlayer> GamePlayer { get; set; }
        public virtual DbSet<Round> Round { get; set; }
        public virtual DbSet<RoundModifier> RoundModifier { get; set; }
        public virtual DbSet<RoundResult> RoundResult { get; set; }
        public virtual DbSet<Team> Team { get; set; }
        public virtual DbSet<TeamAccessToken> TeamAccessToken { get; set; }
        public virtual DbSet<TeamPlayer> TeamPlayer { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(entity =>
            {
                entity.Property(e => e.GameId).ValueGeneratedNever();
            });

            modelBuilder.Entity<GamePlayer>(entity =>
            {
                entity.HasKey(e => new { e.GameId, e.PlayerId });

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.GamePlayer)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("game_player_fk");
            });

            modelBuilder.Entity<Round>(entity =>
            {
                entity.Property(e => e.RoundId).ValueGeneratedNever();

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Round)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("game_round_fk");
            });

            modelBuilder.Entity<RoundModifier>(entity =>
            {
                entity.HasKey(e => new { e.RoundId, e.ModifierType });

                entity.Property(e => e.Contra).HasDefaultValueSql("1");

                entity.Property(e => e.Team).HasDefaultValueSql("1");

                entity.HasOne(d => d.Round)
                    .WithMany(p => p.RoundModifier)
                    .HasForeignKey(d => d.RoundId)
                    .HasConstraintName("game_round_modifier_fk");
            });

            modelBuilder.Entity<RoundResult>(entity =>
            {
                entity.HasKey(e => new { e.GameId, e.RoundId, e.PlayerId });

                entity.HasOne(d => d.Round)
                    .WithMany(p => p.RoundResult)
                    .HasForeignKey(d => d.RoundId)
                    .HasConstraintName("game_round_result_fk");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.Property(e => e.TeamId).ValueGeneratedNever();
            });

            modelBuilder.Entity<TeamAccessToken>(entity =>
            {
                entity.HasKey(e => new { e.TeamId, e.AccessToken });

                entity.Property(e => e.DateIssued).HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<TeamPlayer>(entity =>
            {
                entity.HasKey(e => new { e.TeamId, e.PlayerId });
            });
        }
    }
}
