using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Data.Configurations;

public class ReinvestigationRequestConfiguration : IEntityTypeConfiguration<ReinvestigationRequest>
{
    public void Configure(EntityTypeBuilder<ReinvestigationRequest> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.InitialLodId)
            .IsRequired();
        
        builder.Property(r => r.CaseId)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(r => r.CreatedBy)
            .IsRequired();
        
        builder.Property(r => r.CreatedDate)
            .IsRequired();
        
        builder.Property(r => r.RwoaExplanation)
            .HasMaxLength(int.MaxValue);
        
        // MPF Signature
        builder.Property(r => r.SigNameMpf)
            .HasMaxLength(100);
        
        builder.Property(r => r.SigTitleMpf)
            .HasMaxLength(100);
        
        // Wing JA Signature
        builder.Property(r => r.SigNameWingJa)
            .HasMaxLength(100);
        
        builder.Property(r => r.SigTitleWingJa)
            .HasMaxLength(100);
        
        // Wing CC Signature
        builder.Property(r => r.SigNameWingCc)
            .HasMaxLength(100);
        
        builder.Property(r => r.SigTitleWingCc)
            .HasMaxLength(100);
        
        // Board Admin Signature
        builder.Property(r => r.SigNameBoardAdmin)
            .HasMaxLength(100);
        
        builder.Property(r => r.SigTitleBoardAdmin)
            .HasMaxLength(100);
        
        // Board Medical Signature
        builder.Property(r => r.SigNameBoardMedical)
            .HasMaxLength(100);
        
        builder.Property(r => r.SigTitleBoardMedical)
            .HasMaxLength(100);
        
        // Board Legal Signature
        builder.Property(r => r.SigNameBoardLegal)
            .HasMaxLength(100);
        
        builder.Property(r => r.SigTitleBoardLegal)
            .HasMaxLength(100);
        
        // Approval Signature
        builder.Property(r => r.SigNameApproval)
            .HasMaxLength(100);
        
        builder.Property(r => r.SigTitleApproval)
            .HasMaxLength(100);
        
        // Board Tech Final Signature
        builder.Property(r => r.SigNameBoardTechFinal)
            .HasMaxLength(100);
        
        builder.Property(r => r.SigTitleBoardTechFinal)
            .HasMaxLength(100);
        
        // LOD PM Signature
        builder.Property(r => r.SigNameLodPm)
            .HasMaxLength(100);
        
        builder.Property(r => r.SigTitleLodPm)
            .HasMaxLength(100);
        
        // Board A1 Signature
        builder.Property(r => r.SigNameBoardA1)
            .HasMaxLength(100);
        
        builder.Property(r => r.SigTitleBoardA1)
            .HasMaxLength(100);
        
        // Approval Comments
        builder.Property(r => r.ReturnComment)
            .HasMaxLength(2000);
        
        builder.Property(r => r.WingJaApprovalComment)
            .HasMaxLength(2000);
        
        builder.Property(r => r.WingCcApprovalComment)
            .HasMaxLength(2000);
        
        builder.Property(r => r.BoardTechApproval1Comment)
            .HasMaxLength(1000);
        
        builder.Property(r => r.BoardMedicalApprovalComment)
            .HasMaxLength(2000);
        
        builder.Property(r => r.BoardLegalApprovalComment)
            .HasMaxLength(2000);
        
        builder.Property(r => r.AaFinalApprovalComment)
            .HasMaxLength(2000);
        
        builder.Property(r => r.BoardA1ApprovalComment)
            .HasMaxLength(2000);
        
        // Member Information
        builder.Property(r => r.MemberSsn)
            .HasMaxLength(9);
        
        builder.Property(r => r.MemberName)
            .HasMaxLength(100);
        
        builder.Property(r => r.MemberUnit)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(r => r.MemberUnitId)
            .IsRequired();
        
        builder.Property(r => r.MemberCompo)
            .HasMaxLength(1)
            .IsRequired();
        
        builder.Property(r => r.MemberGrade)
            .IsRequired();
        
        // Cancellation Information
        builder.Property(r => r.CancelExplanation)
            .HasMaxLength(1000);
        
        builder.Property(r => r.IsNonDbSignCase)
            .IsRequired();
        
        // Relationships
        builder.HasOne(r => r.InitialLineOfDutyCase)
            .WithMany(l => l.InitialReinvestigationRequests)
            .HasForeignKey(r => r.InitialLodId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(r => r.ReinvestigationLineOfDutyCase)
            .WithMany(l => l.ReinvestigationRequests)
            .HasForeignKey(r => r.LineOfDutyCaseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
