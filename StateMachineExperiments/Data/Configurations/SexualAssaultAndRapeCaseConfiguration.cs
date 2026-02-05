using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Data.Configurations;

public class SexualAssaultAndRapeCaseConfiguration : IEntityTypeConfiguration<SexualAssaultAndRapeCase>
{
    public void Configure(EntityTypeBuilder<SexualAssaultAndRapeCase> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.LineOfDutyCaseId)
            .IsRequired();
        
        builder.Property(s => s.CaseId)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(s => s.Status)
            .IsRequired();
        
        // Member Information
        builder.Property(s => s.MemberName)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(s => s.MemberSsn)
            .HasMaxLength(9)
            .IsRequired();
        
        builder.Property(s => s.MemberGrade)
            .IsRequired();
        
        builder.Property(s => s.MemberUnit)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(s => s.MemberUnitId)
            .IsRequired();
        
        builder.Property(s => s.MemberCompo)
            .HasMaxLength(1)
            .IsRequired();
        
        // Case Details
        builder.Property(s => s.CreatedBy)
            .IsRequired();
        
        builder.Property(s => s.CreatedDate)
            .IsRequired();
        
        // RSL Wing SARC Signature
        builder.Property(s => s.SigNameRslWingSarc)
            .HasMaxLength(100);
        
        builder.Property(s => s.SigTitleRslWingSarc)
            .HasMaxLength(50);
        
        // SARC A1 Signature
        builder.Property(s => s.SigNameSarcA1)
            .HasMaxLength(100);
        
        builder.Property(s => s.SigTitleSarcA1)
            .HasMaxLength(50);
        
        // Board Medical Signature
        builder.Property(s => s.SigNameBoardMedical)
            .HasMaxLength(100);
        
        builder.Property(s => s.SigTitleBoardMedical)
            .HasMaxLength(50);
        
        // Board JA Signature
        builder.Property(s => s.SigNameBoardJa)
            .HasMaxLength(100);
        
        builder.Property(s => s.SigTitleBoardJa)
            .HasMaxLength(50);
        
        // Board Admin Signature
        builder.Property(s => s.SigNameBoardAdmin)
            .HasMaxLength(100);
        
        builder.Property(s => s.SigTitleBoardAdmin)
            .HasMaxLength(50);
        
        // Approving Authority Signature
        builder.Property(s => s.SigNameApproving)
            .HasMaxLength(100);
        
        builder.Property(s => s.SigTitleApproving)
            .HasMaxLength(50);
        
        // RWOA Information
        builder.Property(s => s.RwoaExplanation)
            .HasMaxLength(int.MaxValue);
        
        // Cancellation Information
        builder.Property(s => s.CancelExplanation)
            .HasMaxLength(int.MaxValue);
        
        // Additional Information
        builder.Property(s => s.DefSexAssaultDbCaseNum)
            .HasMaxLength(100);
        
        builder.Property(s => s.ReturnComment)
            .HasMaxLength(2000);
        
        builder.Property(s => s.IsPostProcessingComplete)
            .IsRequired();
        
        // Relationships
        builder.HasOne(s => s.LineOfDutyCase)
            .WithMany(l => l.Sarcs)
            .HasForeignKey(s => s.LineOfDutyCaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
