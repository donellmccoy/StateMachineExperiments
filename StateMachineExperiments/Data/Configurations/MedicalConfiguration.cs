using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Data.Configurations
{
    /// <summary>
    /// Entity configuration for the Medical entity.
    /// </summary>
    public class MedicalConfiguration : IEntityTypeConfiguration<Medical>
    {
        public void Configure(EntityTypeBuilder<Medical> builder)
        {
            builder.HasKey(e => e.Id);
            
            // String property configurations
            builder.Property(e => e.MemberStatus).HasMaxLength(50);
            builder.Property(e => e.EventNatureType).HasMaxLength(100);
            builder.Property(e => e.EventNatureDetails).HasMaxLength(4000);
            builder.Property(e => e.MedicalFacility).HasMaxLength(500);
            builder.Property(e => e.MedicalFacilityType).HasMaxLength(100);
            builder.Property(e => e.DeathInvolvedYn).HasMaxLength(50);
            builder.Property(e => e.MvaInvolvedYn).HasMaxLength(50);
            builder.Property(e => e.PhysicianCancelExplanation).HasMaxLength(4000);
            builder.Property(e => e.DiagnosisText).HasMaxLength(1000);
            builder.Property(e => e.Icd7thChar).HasMaxLength(7);
            builder.Property(e => e.PsychEval).HasMaxLength(10);
            builder.Property(e => e.RelevantCondition).HasMaxLength(500);
            builder.Property(e => e.OtherTest).HasMaxLength(10);
            builder.Property(e => e.DeployedLocation).HasMaxLength(10);
            builder.Property(e => e.MobilityStandards).HasMaxLength(10);
            builder.Property(e => e.MemberCondition).HasMaxLength(10);
            builder.Property(e => e.AlcoholTestDone).HasMaxLength(10);
            builder.Property(e => e.DrugTestDone).HasMaxLength(10);
            builder.Property(e => e.BoardFinalization).HasMaxLength(10);
            builder.Property(e => e.StatusWorsened).HasMaxLength(5);
            
            // Configure one-to-one relationship with LineOfDutyCase
            builder.HasOne(e => e.LineOfDutyCase)
                .WithOne(c => c.Medical)
                .HasForeignKey<Medical>(e => e.LineOfDutyCaseId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configure many-to-one relationship with Member (responsible member)
            builder.HasOne(e => e.MemberResponsible)
                .WithMany()
                .HasForeignKey(e => e.MemberResponsibleId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Configure many-to-one relationship with Member (member from/origin)
            builder.HasOne(e => e.MemberFrom)
                .WithMany()
                .HasForeignKey(e => e.MemberFromId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
