using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Data.Configurations
{
    /// <summary>
    /// Entity configuration for the Finding entity.
    /// </summary>
    public class FindingConfiguration : IEntityTypeConfiguration<Finding>
    {
        public void Configure(EntityTypeBuilder<Finding> builder)
        {
            builder.HasKey(e => e.Id);
            
            // String property configurations
            builder.Property(e => e.Ssn).HasMaxLength(9);
            builder.Property(e => e.Name).HasMaxLength(50);
            builder.Property(e => e.Grade).HasMaxLength(50);
            builder.Property(e => e.Component).HasMaxLength(6);
            builder.Property(e => e.Rank).HasMaxLength(50);
            builder.Property(e => e.PasCode).HasMaxLength(8);
            builder.Property(e => e.DecisionYn).HasMaxLength(1);
            builder.Property(e => e.FindingsText).HasMaxLength(2400);
            builder.Property(e => e.CorrectlyIdentified).HasMaxLength(5);
            builder.Property(e => e.VerifiedAndAttached).HasMaxLength(5);
            builder.Property(e => e.IdtStatus).HasMaxLength(5);
            builder.Property(e => e.StatusWorsened).HasMaxLength(5);
            
            // Configure relationship with LineOfDutyCase
            builder.HasOne(e => e.LineOfDutyCase)
                .WithMany(c => c.Findings)
                .HasForeignKey(e => e.LineOfDutyCaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
