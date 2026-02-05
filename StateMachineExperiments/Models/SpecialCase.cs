namespace StateMachineExperiments.Models;

/// <summary>
/// Represents a Special Case for various medical and personnel processes,
/// including waivers, physical health assessments, and administrative actions.
/// </summary>
public class SpecialCase
{
    public int Id { get; set; }
    
    public byte ModuleId { get; set; }
    
    public int? AssociatedLodId { get; set; }
    
    public required string CaseId { get; set; }
    
    public int CreatedBy { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public int ModifiedBy { get; set; }
    
    public DateTime ModifiedDate { get; set; }
    
    public byte Workflow { get; set; }
    
    public int Status { get; set; }
    
    public int SubWorkflowType { get; set; }
    
    // RWOA Information
    public byte? RwoaReason { get; set; }
    
    public string? RwoaExplanation { get; set; }
    
    public DateTime? RwoaDate { get; set; }
    
    // Med Tech Signature
    public DateTime? SigDateMedTech { get; set; }
    
    public string? SigNameMedTech { get; set; }
    
    public string? SigTitleMedTech { get; set; }
    
    // HQT Signature
    public DateTime? SigDateHqt { get; set; }
    
    public string? SigNameHqt { get; set; }
    
    public string? SigTitleHqt { get; set; }
    
    // Med Off Signature
    public DateTime? SigDateMedOff { get; set; }
    
    public string? SigNameMedOff { get; set; }
    
    public string? SigTitleMedOff { get; set; }
    
    // HQT Final Signature
    public DateTime? SigDateHqtFinal { get; set; }
    
    public string? SigNameHqtFinal { get; set; }
    
    public string? SigTitleHqtFinal { get; set; }
    
    // Unit PH Signature
    public DateTime? SigDateUnitPh { get; set; }
    
    public string? SigNameUnitPh { get; set; }
    
    public string? SigTitleUnitPh { get; set; }
    
    // HQ DPH Signature
    public DateTime? SigDateHqDph { get; set; }
    
    public string? SigNameHqDph { get; set; }
    
    public string? SigTitleHqDph { get; set; }
    
    // POC Signature
    public DateTime? SigDatePoc { get; set; }
    
    public string? SigNamePoc { get; set; }
    
    public string? SigTitlePoc { get; set; }
    
    // DAWG Signature
    public DateTime? SigDateDawg { get; set; }
    
    public string? SigNameDawg { get; set; }
    
    public string? SigTitleDawg { get; set; }
    
    // Approval Flags
    public int? MedTechApproved { get; set; }
    
    public int? HqtApproval1 { get; set; }
    
    public int? MedOffApproved { get; set; }
    
    public int? HqtApproval2 { get; set; }
    
    public int? MedOffConcur { get; set; }
    
    public int? MedOffPrevDisposition { get; set; }
    
    public int? DawgRecommendation { get; set; }
    
    public int? SeniorMedicalReviewerApproved { get; set; }
    
    // Approval Comments
    public string? MedTechApprovalComment { get; set; }
    
    public string? HqtApproval1Comment { get; set; }
    
    public string? MedOffApprovalComment { get; set; }
    
    public string? HqtApproval2Comment { get; set; }
    
    public string? ReturnComment { get; set; }
    
    public string? SeniorMedicalReviewerComment { get; set; }
    
    public string? SeniorMedicalReviewerConcur { get; set; }
    
    // Member Information
    public required string MemberSsn { get; set; }
    
    public required string MemberName { get; set; }
    
    public required string MemberUnit { get; set; }
    
    public int MemberUnitId { get; set; }
    
    public int MemberCompo { get; set; }
    
    public int? MemberGrade { get; set; }
    
    public DateTime? MemberDob { get; set; }
    
    public int? MemberStatusId { get; set; }
    
    public string? MemberStatus { get; set; }
    
    public string? MemberCategory { get; set; }
    
    // Member Address
    public string? MemberAddressStreet { get; set; }
    
    public string? MemberAddressCity { get; set; }
    
    public string? MemberAddressState { get; set; }
    
    public string? MemberAddressZip { get; set; }
    
    public string? MemberHomePhone { get; set; }
    
    public int? MemberTricareRegion { get; set; }
    
    // Case Details
    public int? DocGroupId { get; set; }
    
    public DateTime? ApprovalDate { get; set; }
    
    public DateTime? ExpirationDate { get; set; }
    
    public int? InitialSc { get; set; }
    
    public int? AlcLetterType { get; set; }
    
    public int? AltAlcLetterType { get; set; }
    
    public string? TmtNumber { get; set; }
    
    public DateTime? TmtReceiveDate { get; set; }
    
    public DateTime? SuspenseDate { get; set; }
    
    public DateTime? DateIn { get; set; }
    
    public DateTime? DateOut { get; set; }
    
    public string? CaseComments { get; set; }
    
    // POC Information
    public string? PocUnit { get; set; }
    
    public string? PocPhoneDsn { get; set; }
    
    public string? PocEmail { get; set; }
    
    public string? PocRankAndName { get; set; }
    
    // Unit POC Information
    public string? UnitPocName { get; set; }
    
    public int? UnitPocRank { get; set; }
    
    public string? UnitPocTitle { get; set; }
    
    public string? UnitPocPhone { get; set; }
    
    // IPEB Information
    public int? IpebElection { get; set; }
    
    public DateTime? MuqRequestDate { get; set; }
    
    public DateTime? IpebSignatureDate { get; set; }
    
    public int? IpebRefusal { get; set; }
    
    // Cover Letter Flags
    public int? CoverLtrIncMemberStatement { get; set; }
    
    public int? CoverLtrIncContactAttemptDetails { get; set; }
    
    // Medical Evaluation Dates
    public DateTime? MedEvalFactSheetSignDate { get; set; }
    
    public DateTime? MedEvalFsWaiverSignDate { get; set; }
    
    public DateTime? Ps3811SignDate { get; set; }
    
    public DateTime? FirstClassMailDate { get; set; }
    
    public DateTime? Ps3811RequestDate { get; set; }
    
    // Disposition and Decision
    public int? HqtechDisposition { get; set; }
    
    public string? DeciExpl { get; set; }
    
    // Waiver Information
    public int? PwaiverCategory { get; set; }
    
    public string? PwaiverCategoryText { get; set; }
    
    public int? PwaiverLength { get; set; }
    
    public bool? WaiverRequired { get; set; }
    
    public DateTime? WaiverExpirationDate { get; set; }
    
    // Fast Track Information
    public int? FastTrackType { get; set; }
    
    public int? RequiresSpecialistForMgmt { get; set; }
    
    public int? MissedWorkDays { get; set; }
    
    public int? HadErUrgentCareVisits { get; set; }
    
    public string? HospitalizationList { get; set; }
    
    public int? RiskForSuddenIncapacitation { get; set; }
    
    public int? YearsSatisfactoryService { get; set; }
    
    public string? FtDiagnosis { get; set; }
    
    public int? DxInterferesWithDuties { get; set; }
    
    public string? FtPrognosis { get; set; }
    
    public string? FtTreatment { get; set; }
    
    public string? FtMedicationsAndDosages { get; set; }
    
    public int? RecommendedFollowUpInterval { get; set; }
    
    // Sleep-Related Information
    public int? DaytimeSomnolence { get; set; }
    
    public string? DaySleepDescription { get; set; }
    
    public int? HasApneaEpisodes { get; set; }
    
    public string? ApneaEpisodeDescription { get; set; }
    
    public int? SleepStudyResults { get; set; }
    
    public decimal? BodyMassIndex { get; set; }
    
    public int? OralDevicesUsed { get; set; }
    
    public int? CpapRequired { get; set; }
    
    public int? BipapRequired { get; set; }
    
    public int? ResponseToDevices { get; set; }
    
    // Diabetes-Related Information
    public decimal? FastingBloodSugar { get; set; }
    
    public decimal? HgbA1C { get; set; }
    
    public int? CurrentOptometryExam { get; set; }
    
    public int? HasSignificantConditions { get; set; }
    
    public string? OtherSignificantConditionsList { get; set; }
    
    public int? ControlledWithOralAgents { get; set; }
    
    public string? OralAgentsList { get; set; }
    
    public int? RequiresInsulin { get; set; }
    
    public string? InsulinDosageRegime { get; set; }
    
    public int? RequiresNonInsulinMed { get; set; }
    
    // Pulmonary Information
    public int? PulmonaryFunctionTest { get; set; }
    
    public int? MethacholineChallenge { get; set; }
    
    public int? RequiresDailySteroids { get; set; }
    
    public int? RescueInhalerUsageFrequency { get; set; }
    
    public int? SymptomsExacerbatedByColdOrExercise { get; set; }
    
    public string? ExerciseOrColdExacerbatedSymptomDescription { get; set; }
    
    public int? NormalPftWithTreatment { get; set; }
    
    public int? HoIntubation { get; set; }
    
    public decimal? DailysteroidsDosage { get; set; }
    
    public int? ExacerbatedSymptomsReqOralSteroids { get; set; }
    
    public string? ExacerbatedSymptomsOralSteroidsDosage { get; set; }
    
    // DAFSC Information
    public string? Dafsc { get; set; }
    
    public int? DafscIsSuitable { get; set; }
    
    // Additional Visit/Care Information
    public string? ErUrgentCareVisitDetails { get; set; }
    
    public int? HadHospitalizations { get; set; }
    
    public string? RmuInitials { get; set; }
    
    // Document Upload Flags
    public int? WwdDocsAttached { get; set; }
    
    public int? CoverLetterUploaded { get; set; }
    
    public int? AfForm469Uploaded { get; set; }
    
    public DateTime? MuqUploadDate { get; set; }
    
    public int? UnitCmdrMemoUploaded { get; set; }
    
    public int? PrivatePhysicianDocsUploaded { get; set; }
    
    public int? Ps3811Uploaded { get; set; }
    
    public int? MemberLetterUploaded { get; set; }
    
    public int? NarrativeSummaryUploaded { get; set; }
    
    public int? MuqValid { get; set; }
    
    // ICD Information
    public int? Icd9Id { get; set; }
    
    public string? Icd9Description { get; set; }
    
    public string? Icd7thChar { get; set; }
    
    // Return to Duty
    public DateTime? ReturnToDutyDate { get; set; }
    
    public int? MedGroupName { get; set; }
    
    public int? RmuName { get; set; }
    
    // LOD/SC Association
    public int? HasAdminLod { get; set; }
    
    public int? HasAdminSc { get; set; }
    
    // Disqualification Information
    public string? DqParagraph { get; set; }
    
    public DateTime? DqCompletionDate { get; set; }
    
    // Cancellation Information
    public int? CaseCancelReason { get; set; }
    
    public string? CaseCancelExplanation { get; set; }
    
    public DateTime? CaseCancelDate { get; set; }
    
    // SAF Letter
    public DateTime? SafLetterUploadDate { get; set; }
    
    public int? ApprovingAuthorityType { get; set; }
    
    public DateTime? ForwardDate { get; set; }
    
    public DateTime? NotificationDate { get; set; }
    
    // Return Routing
    public int? ReturnToGroup { get; set; }
    
    public int? ReturnByGroup { get; set; }
    
    // Follow-Up Care
    public string? FollowUpCare { get; set; }
    
    public string? MedicalProvider { get; set; }
    
    public string? MtfSuggested { get; set; }
    
    public string? MilitaryTreatmentFacilityInitial { get; set; }
    
    public DateTime? MtfInitialTreatmentDate { get; set; }
    
    public int? MtfSuggestedDistance { get; set; }
    
    public int? MtfSuggestedChoice { get; set; }
    
    public string? MilitaryTreatmentFacilityCityStateZip { get; set; }
    
    // Medical Profile
    public string? MedicalProfileInfo { get; set; }
    
    public DateTime? InjuryIllnessDate { get; set; }
    
    // Tenure and Template
    public DateTime? HighTenureDate { get; set; }
    
    public int? MemoTemplateId { get; set; }
    
    // Deployment Information
    public int? Majcom { get; set; }
    
    public int? SimDeployment { get; set; }
    
    public DateTime? DeployStartDate { get; set; }
    
    public DateTime? DeployEndDate { get; set; }
    
    public string? DeployLocation { get; set; }
    
    // Line Information
    public string? LineNumber { get; set; }
    
    public string? LineRemarks { get; set; }
    
    // Code37 and Surgery
    public DateTime? Code37InitDate { get; set; }
    
    public DateTime? SurgeryDate { get; set; }
    
    // Associated SC and Case Type
    public int? AssociatedSc { get; set; }
    
    public int? CaseType { get; set; }
    
    public string? Justification { get; set; }
    
    public int? PeppType { get; set; }
    
    public int? Rating { get; set; }
    
    public int? Renewal { get; set; }
    
    public int? BaseAssign { get; set; }
    
    public int? CompletedByUnit { get; set; }
    
    public string? TypeName { get; set; }
    
    public string? RatingName { get; set; }
    
    public DateTime? DateReceived { get; set; }
    
    // Certification
    public DateTime? CertificationDate { get; set; }
    
    public int? SubCaseType { get; set; }
    
    public int? CertificationStamp { get; set; }
    
    public string? FreeText { get; set; }
    
    public string? CompletedByUnitName { get; set; }
    
    public string? CaseTypeName { get; set; }
    
    public string? SubCaseTypeName { get; set; }
    
    public long? StampedDocId { get; set; }
    
    // Physical Health
    public int? PhWingRmuId { get; set; }
    
    public int? PhUserId { get; set; }
    
    public bool? IsDelinquent { get; set; }
    
    public DateTime? PhReportingPeriod { get; set; }
    
    public DateTime? PhLastModified { get; set; }
    
    // Secondary Certification
    public int? SecondaryCertificationStamp { get; set; }
    
    public string? SecondaryFreeText { get; set; }
    
    // Process and Renewal
    public int? Process { get; set; }
    
    public DateTime? RenewalDate { get; set; }
    
    // Alternate Information (duplicates with "Alternate_" prefix)
    public int? AlternateAlcLetterType { get; set; }
    
    public int? AlternateAltAlcLetterType { get; set; }
    
    public DateTime? AlternateApprovalDate { get; set; }
    
    public DateTime? AlternateCertificationDate { get; set; }
    
    public DateTime? AlternateDqCompletionDate { get; set; }
    
    public string? AlternateDqParagraph { get; set; }
    
    public DateTime? AlternateExpirationDate { get; set; }
    
    public int? AlternateMedOffConcur { get; set; }
    
    public int? AlternateMemoTemplateId { get; set; }
    
    public int? AlternateProcess { get; set; }
    
    public int? AlternatePwaiverLength { get; set; }
    
    public DateTime? AlternateReturnToDutyDate { get; set; }
    
    // PHA Information
    public DateTime? PhaDate { get; set; }
    
    public int? InitialTour { get; set; }
    
    public int? FollowOnTour { get; set; }
    
    // IAW AFI Flag
    public bool? IawAfi { get; set; }
    
    // Accident or History Details
    public string? AccidentOrHistoryDetails { get; set; }
    
    // Navigation Properties
    // =====================
    
    public LineOfDutyCase? LineOfDutyCase { get; set; }
}
