using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachineExperiments.Enums;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Data.Configurations
{
    /// <summary>
    /// Entity configuration for the LineOfDutyCase entity.
    /// </summary>
    public class LineOfDutyCaseConfiguration : IEntityTypeConfiguration<LineOfDutyCase>
    {
        public void Configure(EntityTypeBuilder<LineOfDutyCase> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.HasIndex(e => e.CaseNumber).IsUnique();
            
            // Configure discriminator column
            builder.HasDiscriminator(e => e.LineOfDutyType)
                .HasValue<LineOfDutyCase>(LineOfDutyType.Informal)
                .HasValue<LineOfDutyCase>(LineOfDutyType.Formal);
            
            // Common properties
            builder.Property(e => e.CaseNumber)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(e => e.LineOfDutyState)
                .IsRequired()
                .HasMaxLength(50);
            
            // Formal-specific properties
            builder.Property(e => e.InvestigatingOfficerId)
                .HasMaxLength(50);
            
            builder.Property(e => e.InvestigatingOfficerName)
                .HasMaxLength(200);
            
            builder.Property(e => e.DeterminationResult)
                .HasMaxLength(100);
            
            // Configure RowVersion for concurrency
            builder.Property(e => e.RowVersion)
                .IsRowVersion();
            
            // Configure relationship with Member
            builder.HasOne(e => e.Member)
                .WithMany(m => m.LineOfDutyCases)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
