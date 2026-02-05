using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Data.Configurations;

public class SpecialCaseConfiguration : IEntityTypeConfiguration<SpecialCase>
{
    public void Configure(EntityTypeBuilder<SpecialCase> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.ModuleId)
            .IsRequired();
        
        builder.Property(s => s.CaseId)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(s => s.CreatedBy)
            .IsRequired();
        
        builder.Property(s => s.CreatedDate)
            .IsRequired();
        
        builder.Property(s => s.ModifiedBy)
            .IsRequired();
        
        builder.Property(s => s.ModifiedDate)
            .IsRequired();
        
        builder.Property(s => s.Workflow)
            .IsRequired();
        
        builder.Property(s => s.Status)
            .IsRequired();
        
        builder.Property(s => s.SubWorkflowType)
            .IsRequired();
        
        // RWOA
        builder.Property(s => s.RwoaExplanation)
            .HasMaxLength(int.MaxValue);
        
        // Signature Names and Titles
        builder.Property(s => s.SigNameMedTech).HasMaxLength(100);
        builder.Property(s => s.SigTitleMedTech).HasMaxLength(100);
        builder.Property(s => s.SigNameHqt).HasMaxLength(100);
        builder.Property(s => s.SigTitleHqt).HasMaxLength(100);
        builder.Property(s => s.SigNameMedOff).HasMaxLength(100);
        builder.Property(s => s.SigTitleMedOff).HasMaxLength(100);
        builder.Property(s => s.SigNameHqtFinal).HasMaxLength(100);
        builder.Property(s => s.SigTitleHqtFinal).HasMaxLength(100);
        builder.Property(s => s.SigNameUnitPh).HasMaxLength(100);
        builder.Property(s => s.SigTitleUnitPh).HasMaxLength(100);
        builder.Property(s => s.SigNameHqDph).HasMaxLength(100);
        builder.Property(s => s.SigTitleHqDph).HasMaxLength(100);
        builder.Property(s => s.SigNamePoc).HasMaxLength(100);
        builder.Property(s => s.SigTitlePoc).HasMaxLength(100);
        builder.Property(s => s.SigNameDawg).HasMaxLength(100);
        builder.Property(s => s.SigTitleDawg).HasMaxLength(100);
        
        // Approval Comments
        builder.Property(s => s.MedTechApprovalComment).HasMaxLength(500);
        builder.Property(s => s.HqtApproval1Comment).HasMaxLength(500);
        builder.Property(s => s.MedOffApprovalComment).HasMaxLength(2000);
        builder.Property(s => s.HqtApproval2Comment).HasMaxLength(250);
        builder.Property(s => s.ReturnComment).HasMaxLength(2000);
        builder.Property(s => s.SeniorMedicalReviewerComment).HasMaxLength(2000);
        builder.Property(s => s.SeniorMedicalReviewerConcur).HasMaxLength(1);
        
        // Member Information
        builder.Property(s => s.MemberSsn)
            .HasMaxLength(9)
            .IsRequired();
        
        builder.Property(s => s.MemberName)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(s => s.MemberUnit)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(s => s.MemberUnitId)
            .IsRequired();
        
        builder.Property(s => s.MemberCompo)
            .IsRequired();
        
        builder.Property(s => s.MemberStatus).HasMaxLength(50);
        builder.Property(s => s.MemberCategory).HasMaxLength(100);
        
        // Member Address
        builder.Property(s => s.MemberAddressStreet).HasMaxLength(400);
        builder.Property(s => s.MemberAddressCity).HasMaxLength(80);
        builder.Property(s => s.MemberAddressState).HasMaxLength(2);
        builder.Property(s => s.MemberAddressZip).HasMaxLength(10);
        builder.Property(s => s.MemberHomePhone).HasMaxLength(20);
        
        // Case Details
        builder.Property(s => s.TmtNumber).HasMaxLength(50);
        builder.Property(s => s.CaseComments).HasMaxLength(1200);
        
        // POC Information
        builder.Property(s => s.PocUnit).HasMaxLength(50);
        builder.Property(s => s.PocPhoneDsn).HasMaxLength(40);
        builder.Property(s => s.PocEmail).HasMaxLength(200);
        builder.Property(s => s.PocRankAndName).HasMaxLength(200);
        
        // Unit POC
        builder.Property(s => s.UnitPocName).HasMaxLength(200);
        builder.Property(s => s.UnitPocTitle).HasMaxLength(200);
        builder.Property(s => s.UnitPocPhone).HasMaxLength(100);
        
        // Waiver
        builder.Property(s => s.PwaiverCategoryText).HasMaxLength(50);
        
        // Fast Track
        builder.Property(s => s.HospitalizationList).HasMaxLength(int.MaxValue);
        builder.Property(s => s.FtDiagnosis).HasMaxLength(int.MaxValue);
        builder.Property(s => s.FtPrognosis).HasMaxLength(int.MaxValue);
        builder.Property(s => s.FtTreatment).HasMaxLength(int.MaxValue);
        builder.Property(s => s.FtMedicationsAndDosages).HasMaxLength(int.MaxValue);
        
        // Sleep
        builder.Property(s => s.DaySleepDescription).HasMaxLength(int.MaxValue);
        builder.Property(s => s.ApneaEpisodeDescription).HasMaxLength(int.MaxValue);
        
        // Diabetes
        builder.Property(s => s.OtherSignificantConditionsList).HasMaxLength(int.MaxValue);
        builder.Property(s => s.OralAgentsList).HasMaxLength(int.MaxValue);
        builder.Property(s => s.InsulinDosageRegime).HasMaxLength(500);
        
        // Pulmonary
        builder.Property(s => s.ExerciseOrColdExacerbatedSymptomDescription).HasMaxLength(int.MaxValue);
        builder.Property(s => s.ExacerbatedSymptomsOralSteroidsDosage).HasMaxLength(int.MaxValue);
        
        // DAFSC
        builder.Property(s => s.Dafsc).HasMaxLength(50);
        
        // Additional
        builder.Property(s => s.ErUrgentCareVisitDetails).HasMaxLength(int.MaxValue);
        builder.Property(s => s.RmuInitials).HasMaxLength(5);
        
        // ICD
        builder.Property(s => s.Icd9Description).HasMaxLength(int.MaxValue);
        builder.Property(s => s.Icd7thChar).HasMaxLength(7);
        
        // DQ
        builder.Property(s => s.DqParagraph).HasMaxLength(500);
        builder.Property(s => s.AlternateDqParagraph).HasMaxLength(500);
        
        // Cancellation
        builder.Property(s => s.CaseCancelExplanation).HasMaxLength(int.MaxValue);
        
        // Follow-Up
        builder.Property(s => s.FollowUpCare).HasMaxLength(2000);
        builder.Property(s => s.MedicalProvider).HasMaxLength(200);
        builder.Property(s => s.MtfSuggested).HasMaxLength(400);
        builder.Property(s => s.MilitaryTreatmentFacilityInitial).HasMaxLength(400);
        builder.Property(s => s.MilitaryTreatmentFacilityCityStateZip).HasMaxLength(200);
        
        // Medical Profile
        builder.Property(s => s.MedicalProfileInfo).HasMaxLength(1000);
        
        // Deployment
        builder.Property(s => s.DeployLocation).HasMaxLength(500);
        
        // Line
        builder.Property(s => s.LineNumber).HasMaxLength(50);
        builder.Property(s => s.LineRemarks).HasMaxLength(500);
        
        // Case Type
        builder.Property(s => s.Justification).HasMaxLength(500);
        builder.Property(s => s.TypeName).HasMaxLength(100);
        builder.Property(s => s.RatingName).HasMaxLength(100);
        
        // Certification
        builder.Property(s => s.FreeText).HasMaxLength(2000);
        builder.Property(s => s.CompletedByUnitName).HasMaxLength(100);
        builder.Property(s => s.CaseTypeName).HasMaxLength(100);
        builder.Property(s => s.SubCaseTypeName).HasMaxLength(100);
        builder.Property(s => s.SecondaryFreeText).HasMaxLength(2000);
        
        // Decision
        builder.Property(s => s.DeciExpl).HasMaxLength(250);
        
        // Accident
        builder.Property(s => s.AccidentOrHistoryDetails).HasMaxLength(1000);
        
        // Decimal properties with precision
        builder.Property(s => s.BodyMassIndex).HasPrecision(18, 2);
        builder.Property(s => s.FastingBloodSugar).HasPrecision(18, 2);
        builder.Property(s => s.HgbA1C).HasPrecision(18, 2);
        builder.Property(s => s.DailysteroidsDosage).HasPrecision(18, 2);
        
        // Relationships
        builder.HasOne(s => s.LineOfDutyCase)
            .WithMany(l => l.SpecialCases)
            .HasForeignKey(s => s.AssociatedLodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
