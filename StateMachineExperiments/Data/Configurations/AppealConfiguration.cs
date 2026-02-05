using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Data.Configurations
{
    /// <summary>
    /// Entity configuration for the Appeal entity.
    /// </summary>
    public class AppealConfiguration : IEntityTypeConfiguration<Appeal>
    {
        public void Configure(EntityTypeBuilder<Appeal> builder)
        {
            builder.HasKey(e => e.Id);
            
            // Required string properties
            builder.Property(e => e.CaseId)
                .IsRequired()
                .HasMaxLength(50);
                        
            builder.Property(e => e.MemberCompo)
                .IsRequired()
                .HasMaxLength(1);
            
            // Optional string properties with max length
            builder.Property(e => e.CancelExplanation).HasMaxLength(1000);
            builder.Property(e => e.SignatureNamePm).HasMaxLength(100);
            builder.Property(e => e.SignatureTitlePm).HasMaxLength(100);
            builder.Property(e => e.SignatureNameBoardTech).HasMaxLength(100);
            builder.Property(e => e.SignatureTitleBoardTech).HasMaxLength(100);
            builder.Property(e => e.SignatureNameBoardMedical).HasMaxLength(100);
            builder.Property(e => e.SignatureTitleBoardMedical).HasMaxLength(100);
            builder.Property(e => e.SignatureNameBoardLegal).HasMaxLength(100);
            builder.Property(e => e.SignatureTitleBoardLegal).HasMaxLength(100);
            builder.Property(e => e.SignatureNameBoardAdmin).HasMaxLength(100);
            builder.Property(e => e.SignatureTitleBoardAdmin).HasMaxLength(100);
            builder.Property(e => e.SignatureNameApprovingAuth).HasMaxLength(100);
            builder.Property(e => e.SignatureTitleApprovingAuth).HasMaxLength(100);
            builder.Property(e => e.SignatureNameAppellateAuth).HasMaxLength(100);
            builder.Property(e => e.SignatureTitleAppellateAuth).HasMaxLength(100);
            builder.Property(e => e.LodPmApprovalComment).HasMaxLength(2000);
            builder.Property(e => e.BoardTechApprovalComment).HasMaxLength(2000);
            builder.Property(e => e.BoardMedicalApprovalComment).HasMaxLength(2000);
            builder.Property(e => e.BoardLegalApprovalComment).HasMaxLength(2000);
            builder.Property(e => e.BoardAdminApprovalComment).HasMaxLength(2000);
            builder.Property(e => e.ApprovalAuthApprovalComment).HasMaxLength(2000);
            builder.Property(e => e.AppellateAuthApprovalComment).HasMaxLength(2000);
            builder.Property(e => e.ReturnComment).HasMaxLength(2000);
            builder.Property(e => e.MemberSsn).HasMaxLength(9);
            builder.Property(e => e.MemberName).HasMaxLength(100);
            
            // Configure relationship with LineOfDutyCase
            builder.HasOne(e => e.LineOfDutyCase)
                .WithMany(c => c.Appeals)
                .HasForeignKey(e => e.LineOfDutyCaseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
