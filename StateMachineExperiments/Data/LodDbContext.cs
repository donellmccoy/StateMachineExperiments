using Microsoft.EntityFrameworkCore;
using StateMachineExperiments.Data.Configurations;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Data
{
    public class LodDbContext : DbContext
    {
        public DbSet<LineOfDutyCase> LineOfDutyCases { get; set; } = null!;

        public DbSet<LineOfDutyStateTransitionHistory> TransitionHistory { get; set; } = null!;
        
        public DbSet<Member> Members { get; set; } = null!;
        
        public DbSet<Medical> Medicals { get; set; } = null!;
        
        public DbSet<Unit> Units { get; set; } = null!;

        public LodDbContext(DbContextOptions<LodDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations
            modelBuilder.ApplyConfiguration(new MemberConfiguration());
            modelBuilder.ApplyConfiguration(new LineOfDutyCaseConfiguration());
            modelBuilder.ApplyConfiguration(new LineOfDutyStateTransitionHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new MedicalConfiguration());
            modelBuilder.ApplyConfiguration(new UnitConfiguration());
        }
    }
}
