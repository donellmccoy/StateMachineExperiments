using Microsoft.EntityFrameworkCore;
using StateMachineExperiments.Modules.InformalLOD.Models;
using StateMachineExperiments.Modules.FormalLOD.Models;

namespace StateMachineExperiments.Common.Data
{
    public class LodDbContext : DbContext
    {
        public DbSet<InformalLineOfDuty> LodCases { get; set; } = null!;

        public DbSet<StateTransitionHistory> TransitionHistory { get; set; } = null!;

        public DbSet<FormalLineOfDuty> FormalLodCases { get; set; } = null!;

        public DbSet<FormalStateTransitionHistory> FormalTransitionHistory { get; set; } = null!;

        public LodDbContext(DbContextOptions<LodDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure LodCase
            modelBuilder.Entity<InformalLineOfDuty>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.CaseNumber).IsUnique();
                entity.Property(e => e.CaseNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CurrentState).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MemberName).HasMaxLength(200);
                entity.Property(e => e.MemberId).HasMaxLength(50);
            });

            // Configure StateTransitionHistory
            modelBuilder.Entity<StateTransitionHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FromState).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ToState).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Trigger).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PerformedByAuthority).HasMaxLength(100);
                
                // Configure relationship
                entity.HasOne(e => e.LodCase)
                    .WithMany(c => c.TransitionHistory)
                    .HasForeignKey(e => e.LodCaseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure FormalLineOfDuty
            modelBuilder.Entity<FormalLineOfDuty>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.CaseNumber).IsUnique();
                entity.Property(e => e.CaseNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CurrentState).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MemberName).HasMaxLength(200);
                entity.Property(e => e.MemberId).HasMaxLength(50);
                entity.Property(e => e.InvestigatingOfficerId).HasMaxLength(50);
                entity.Property(e => e.InvestigatingOfficerName).HasMaxLength(200);
                entity.Property(e => e.DeterminationResult).HasMaxLength(100);
            });

            // Configure FormalStateTransitionHistory
            modelBuilder.Entity<FormalStateTransitionHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FromState).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ToState).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Trigger).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PerformedByAuthority).HasMaxLength(100);
                
                // Configure relationship
                entity.HasOne(e => e.FormalLodCase)
                    .WithMany(c => c.TransitionHistory)
                    .HasForeignKey(e => e.FormalLodCaseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
