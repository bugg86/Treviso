using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Treviso.Domain.Sql.Contexts.Interfaces;
using Treviso.Domain.Sql.Models;

namespace Treviso.Domain.Sql.Contexts;

public class TrevisoEnvContextFactory : IDesignTimeDbContextFactory<TrevisoContext>
{
    public TrevisoContext CreateDbContext(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<TrevisoContext>();
        optionsBuilder.UseSqlServer(config.GetConnectionString("TREVISO_CONNECTION"));

        return new TrevisoContext(optionsBuilder.Options);
    }
}

public partial class TrevisoContext : DbContext, ITrevisoContext
{
    public TrevisoContext(DbContextOptions<TrevisoContext> options) : base(options) { }
    
    public virtual DbSet<Tournament> Tournaments { get; set; } = null!;
    public virtual DbSet<Sheet> Sheets { get; set; } = null!;
    public virtual DbSet<Match> Matches { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TournamentId");
            
            entity.ToTable("tournaments");
            
            entity.Property(e => e.GuildId).HasColumnType("numeric(20, 0)");
            entity.Property(e => e.Abbreviation).HasMaxLength(10);
            entity.Property(e => e.Badged).HasConversion<int>();
            entity.Property(e => e.Bws).HasConversion<int>();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RangeLower).HasMaxLength(10);
            entity.Property(e => e.RangeUpper).HasMaxLength(10);
            entity.Property(e => e.TeamSize).HasColumnType("int");
            entity.Property(e => e.Vs).HasMaxLength(10);
            entity.Property(e => e.User).HasColumnType("numeric(20, 0)");
            entity.Property(e => e.Version).HasColumnType("int");
        });

        modelBuilder.Entity<Sheet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SheetId");
            entity.HasOne(s => s.Tournament)
                .WithOne(t => t.Sheet)
                .HasForeignKey<Sheet>(s => s.TournamentId)
                .HasConstraintName("FK_SheetToTournament");

            entity.ToTable("sheets");

            entity.Property(e => e.Main).HasMaxLength(100);
            entity.Property(e => e.Ref).HasMaxLength(100);
            entity.Property(e => e.RefType).HasMaxLength(20);
            entity.Property(e => e.Pool).HasMaxLength(100);
            entity.Property(e => e.Admin).HasMaxLength(100);
            entity.Property(e => e.User).HasColumnType("numeric(20, 0)");
            entity.Property(e => e.Version).HasColumnType("int");
        });
        
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_MatchId");
            entity.HasOne(m => m.Tournament)
                .WithMany(t => t.Matches)
                .HasForeignKey(m => m.TournamentId)
                .HasConstraintName("FK_MatchToTournament");
            
            entity.ToTable("matches");

            entity.Property(e => e.MatchId).HasMaxLength(20);
            entity.Property(e => e.Round).HasMaxLength(20);
            entity.Property(e => e.Date).HasMaxLength(20);
            entity.Property(e => e.Time).HasMaxLength(20);
            entity.Property(e => e.Team1).HasMaxLength(50);
            entity.Property(e => e.CaptainDiscord1).HasMaxLength(50);
            entity.Property(e => e.Team2).HasMaxLength(50);
            entity.Property(e => e.CaptainDiscord2).HasMaxLength(50);
            entity.Property(e => e.Referee).HasMaxLength(50);
            entity.Property(e => e.RefereeDiscord).HasMaxLength(50);
            entity.Property(e => e.Streamer).HasMaxLength(50);
            entity.Property(e => e.Commentator1).HasMaxLength(50);
            entity.Property(e => e.Commentator2).HasMaxLength(50);
            entity.Property(e => e.PingSent).HasConversion<int>();
            entity.Property(e => e.MatchFinished).HasConversion<int>();
            entity.Property(e => e.User).HasColumnType("numeric(20, 0)");
            entity.Property(e => e.Version).HasColumnType("int");
        });
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}