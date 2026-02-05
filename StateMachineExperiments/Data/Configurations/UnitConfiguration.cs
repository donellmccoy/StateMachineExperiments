using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Data.Configurations
{
    /// <summary>
    /// Entity configuration for the Unit entity.
    /// </summary>
    public class UnitConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.HasKey(e => e.Id);
            
            // String property configurations
            builder.Property(e => e.CommanderCircumstanceDetails).HasMaxLength(1460);
            builder.Property(e => e.CommanderDutyDetermination).HasMaxLength(100);
            builder.Property(e => e.CommanderDutyOthers).HasMaxLength(100);
            builder.Property(e => e.CommanderActivatedYn).HasMaxLength(1);
            builder.Property(e => e.SourceInformationSpecify).HasMaxLength(100);
            builder.Property(e => e.MemberOnOrders).HasMaxLength(10);
            builder.Property(e => e.MemberCredible).HasMaxLength(10);
            builder.Property(e => e.ProximateCauseSpecify).HasMaxLength(100);
            builder.Property(e => e.StatusWorsened).HasMaxLength(5);
            builder.Property(e => e.StatusWhenInjured).HasMaxLength(20);
            builder.Property(e => e.StatusWhenInjuredExplanation).HasMaxLength(50);
            builder.Property(e => e.OrdersAttached).HasMaxLength(5);
            builder.Property(e => e.IdtStatus).HasMaxLength(5);
            builder.Property(e => e.UtapsAttached).HasMaxLength(5);
            
            // Configure one-to-one relationship with LineOfDutyCase
            builder.HasOne(e => e.LineOfDutyCase)
                .WithOne(c => c.Unit)
                .HasForeignKey<Unit>(e => e.LineOfDutyCaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
