namespace StateMachineExperiments.Modules.FormalLOD.Models
{
    /// <summary>
    /// Represents the possible states in a Formal Line of Duty workflow.
    /// Based on DAFI 36-2910 for AFRC Formal LOD with questionable circumstances.
    /// </summary>
    public enum FormalLodState
    {
        /// <summary>Initial state before processing begins.</summary>
        Start,
        /// <summary>Member has reported a condition.</summary>
        MemberReports,
        /// <summary>Formal LOD process has been initiated due to questionable circumstances.</summary>
        FormalInitiation,
        /// <summary>Appointing authority is assigning an investigating officer.</summary>
        AppointingOfficer,
        /// <summary>Investigating officer is conducting the investigation.</summary>
        Investigation,
        /// <summary>Wing legal review is in progress.</summary>
        WingLegalReview,
        /// <summary>Wing commander review is in progress.</summary>
        WingCommanderReview,
        /// <summary>Board adjudication in progress.</summary>
        BoardAdjudication,
        /// <summary>Final determination has been made.</summary>
        Determination,
        /// <summary>Member is being notified of the determination.</summary>
        Notification,
        /// <summary>Appeal has been filed and is being processed.</summary>
        Appeal,
        /// <summary>Final state - case is closed.</summary>
        End
    }

    /// <summary>
    /// Represents the triggers that cause state transitions in the Formal LOD workflow.
    /// </summary>
    public enum FormalLodTrigger
    {
        /// <summary>Initiates the LOD process.</summary>
        ProcessInitiated,
        /// <summary>Member reports a condition.</summary>
        ConditionReported,
        /// <summary>Questionable circumstances detected requiring formal investigation.</summary>
        QuestionableDetected,
        /// <summary>Investigating officer has been appointed.</summary>
        OfficerAppointed,
        /// <summary>Investigation is complete (DD Form 261).</summary>
        InvestigationComplete,
        /// <summary>Legal review is complete.</summary>
        LegalReviewComplete,
        /// <summary>Wing commander review is complete.</summary>
        WingReviewComplete,
        /// <summary>Adjudication is complete.</summary>
        AdjudicationComplete,
        /// <summary>Determination has been finalized.</summary>
        DeterminationFinalized,
        /// <summary>Member has filed an appeal.</summary>
        AppealRequested,
        /// <summary>Appeal has been resolved.</summary>
        AppealResolved,
        /// <summary>No appeal was filed within the time limit.</summary>
        NoAppealRequested
    }

    /// <summary>
    /// Represents the authority levels involved in Formal LOD processing.
    /// Based on DAFI 36-2910 for AFRC Formal LOD.
    /// </summary>
    public enum FormalLodAuthority
    {
        None,
        Member,                 // The reporting member
        LodMfp,                 // LOD-MFP (Medical Force Provider)
        MedicalProvider,        // Military Medical Provider (initial assessment)
        AppointingAuthority,    // Senior AFR CC or HQ RIO/CC (appoints investigating officer)
        InvestigatingOfficer,   // Appointed O-3+ officer
        LegalAdvisor,           // Wing Staff Judge Advocate
        WingCommander,          // Wing CC or equivalent
        ReviewingBoard,         // AFRC LOD Determination Board (SG, JA, A1)
        ApprovingAuthority,     // HQ AFRC/A1
        LodPm,                  // LOD Program Manager (notifications)
        AppellateAuthority      // HQ AFRC/CD (appeals)
    }
}
