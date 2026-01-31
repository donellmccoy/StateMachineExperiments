namespace StateMachineExperiments.Modules.InformalLOD.Models
{
    /// <summary>
    /// Represents the possible states in an Informal Line of Duty workflow.
    /// </summary>
    public enum LodState
    {
        /// <summary>Initial state before processing begins.</summary>
        Start,
        /// <summary>Member has reported a condition.</summary>
        MemberReports,
        /// <summary>LOD initiation process is underway.</summary>
        LodInitiation,
        /// <summary>Medical assessment is in progress.</summary>
        MedicalAssessment,
        /// <summary>Commander is reviewing the case.</summary>
        CommanderReview,
        /// <summary>Optional legal review state.</summary>
        OptionalLegal,
        /// <summary>Optional wing commander review state.</summary>
        OptionalWing,
        /// <summary>Board adjudication in progress.</summary>
        BoardAdjudication,
        /// <summary>Determination has been made.</summary>
        Determination,
        /// <summary>Member is being notified of the determination.</summary>
        Notification,
        /// <summary>Appeal has been filed and is being processed.</summary>
        Appeal,
        /// <summary>Final state - case is closed.</summary>
        End
    }

    /// <summary>
    /// Represents the triggers that cause state transitions in the LOD workflow.
    /// </summary>
    public enum LodTrigger
    {
        /// <summary>Initiates the LOD process.</summary>
        ProcessInitiated,
        /// <summary>Member reports a condition.</summary>
        ConditionReported,
        /// <summary>Initiation documentation is complete.</summary>
        InitiationComplete,
        /// <summary>Medical assessment is finished.</summary>
        AssessmentDone,
        /// <summary>Commander review is complete.</summary>
        ReviewFinished,
        /// <summary>Skip optional reviews and go to adjudication.</summary>
        SkipToAdjudication,
        /// <summary>Legal review is complete.</summary>
        LegalDone,
        /// <summary>Skip wing review after legal.</summary>
        SkipWingReview,
        /// <summary>Wing review is complete.</summary>
        WingDone,
        /// <summary>Adjudication is complete.</summary>
        AdjudicationComplete,
        /// <summary>Determination has been finalized.</summary>
        DeterminationFinalized,
        /// <summary>Notification complete without appeal.</summary>
        NotificationComplete,
        /// <summary>Member has filed an appeal.</summary>
        AppealFiled,
        /// <summary>Appeal has been resolved.</summary>
        AppealResolved
    }

    /// <summary>
    /// Represents the authority levels involved in LOD processing.
    /// Based on DAFI 36-2910 for AFRC Informal LOD.
    /// </summary>
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
