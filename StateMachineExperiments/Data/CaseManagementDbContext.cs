using Microsoft.EntityFrameworkCore;
using StateMachineExperiments.Data.Configurations;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Data
{
    public class CaseManagementDbContext : DbContext
    {
        public DbSet<LineOfDutyCase> LineOfDutyCases { get; set; } = null!;

        public DbSet<LineOfDutyStateTransitionHistory> TransitionHistory { get; set; } = null!;
        
        public DbSet<Member> Members { get; set; } = null!;
        
        public DbSet<Medical> Medicals { get; set; } = null!;
        
        public DbSet<Unit> Units { get; set; } = null!;
        
        public DbSet<Appeal> Appeals { get; set; } = null!;
        
        public DbSet<Comment> Comments { get; set; } = null!;
        
        public DbSet<Finding> Findings { get; set; } = null!;
        
        public DbSet<PostProcessing> PostProcessings { get; set; } = null!;
        
        public DbSet<ReinvestigationRequest> ReinvestigationRequests { get; set; } = null!;
        
        public DbSet<SexualAssaultAndRapeCase> SexualAssaultAndRapeCases { get; set; } = null!;
        
        public DbSet<SpecialCase> SpecialCases { get; set; } = null!;

        public CaseManagementDbContext(DbContextOptions<CaseManagementDbContext> options) : base(options)
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
            modelBuilder.ApplyConfiguration(new AppealConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new FindingConfiguration());
            modelBuilder.ApplyConfiguration(new PostProcessingConfiguration());
            modelBuilder.ApplyConfiguration(new ReinvestigationRequestConfiguration());
            modelBuilder.ApplyConfiguration(new SexualAssaultAndRapeCaseConfiguration());
            modelBuilder.ApplyConfiguration(new SpecialCaseConfiguration());
        }
    }
}
