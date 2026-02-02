using Microsoft.EntityFrameworkCore;
using StateMachineExperiments.Models;
using StateMachineExperiments.Enums;

namespace StateMachineExperiments.Data
{
    public class LodDbContext : DbContext
    {
        public DbSet<LineOfDuty> LineOfDutyCases { get; set; } = null!;

        public DbSet<LodStateTransitionHistory> TransitionHistory { get; set; } = null!;

        public LodDbContext(DbContextOptions<LodDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure LineOfDuty with discriminator for Informal/Formal types
            modelBuilder.Entity<LineOfDuty>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.CaseNumber).IsUnique();
                
                // Configure discriminator column
                entity.HasDiscriminator(e => e.CaseType)
                    .HasValue<LineOfDuty>(LodType.Informal)
                    .HasValue<LineOfDuty>(LodType.Formal);
                
                // Common properties
                entity.Property(e => e.CaseNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CurrentState).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MemberName).HasMaxLength(200);
                entity.Property(e => e.MemberId).HasMaxLength(50);
                
                // Informal-specific properties
                // These are nullable by design and only used when CaseType == Informal
                
                // Formal-specific properties
                entity.Property(e => e.InvestigatingOfficerId).HasMaxLength(50);
                entity.Property(e => e.InvestigatingOfficerName).HasMaxLength(200);
                entity.Property(e => e.DeterminationResult).HasMaxLength(100);
            });

            // Configure LodStateTransitionHistory
            modelBuilder.Entity<LodStateTransitionHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FromState).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ToState).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Trigger).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PerformedByAuthority).HasMaxLength(100);
                
                // Configure relationship
                entity.HasOne(e => e.LineOfDutyCase)
                    .WithMany(c => c.TransitionHistory)
                    .HasForeignKey(e => e.LineOfDutyCaseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
