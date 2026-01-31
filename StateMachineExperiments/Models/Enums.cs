namespace StateMachineExperiments.Models
{
    // Define the states as an enum
    public enum LodState
    {
        Start,
        MemberReports,
        LodInitiation,
        MedicalAssessment,
        CommanderReview,
        OptionalLegal,
        OptionalWing,
        BoardAdjudication,
        Determination,
        Notification,
        Appeal,
        End
    }

    // Define triggers as an enum
    public enum LodTrigger
    {
        ProcessInitiated,
        ConditionReported,
        InitiationComplete,
        AssessmentDone,
        ReviewFinished,
        LegalDone,
        WingDone,
        AdjudicationComplete,
        DeterminationFinalized,
        AppealRequested,
        AppealResolved,
        NoAppealRequested
    }

    // Enum for Authorities based on DAFI 36-2910 for AFRC Informal LOD
    public enum LodAuthority
    {
        None,
        Member,                 // The reporting member
        LodMfp,                 // LOD-MFP (e.g., Medical Force Provider)
        MedicalProvider,        // Military Medical Provider
        ImmediateCommander,     // First full-time CC or Detachment CC
        LegalAdvisor,           // Wing Staff Judge Advocate (optional)
        WingCommander,          // Wing CC or equivalent (optional)
        ReviewingBoard,         // AFRC LOD Determination Board (SG, JA, A1)
        ApprovingAuthority,     // HQ AFRC/A1
        LodPm,                  // LOD Program Manager (for notifications)
        AppellateAuthority      // HQ AFRC/CD (for appeals)
    }
}
