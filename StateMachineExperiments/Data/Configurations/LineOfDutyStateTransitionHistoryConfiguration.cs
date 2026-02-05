using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Data.Configurations
{
    /// <summary>
    /// Entity configuration for the LineOfDutyStateTransitionHistory entity.
    /// </summary>
    public class LineOfDutyStateTransitionHistoryConfiguration : IEntityTypeConfiguration<LineOfDutyStateTransitionHistory>
    {
        public void Configure(EntityTypeBuilder<LineOfDutyStateTransitionHistory> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.FromState)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(e => e.ToState)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(e => e.Trigger)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(e => e.PerformedByAuthority)
                .HasMaxLength(100);
            
            // Configure relationship
            builder.HasOne(e => e.LineOfDutyCase)
                .WithMany(c => c.TransitionHistory)
                .HasForeignKey(e => e.LineOfDutyCaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
