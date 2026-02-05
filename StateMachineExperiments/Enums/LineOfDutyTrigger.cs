namespace StateMachineExperiments.Enums
{
    /// <summary>
    /// Represents the triggers that cause state transitions in LOD workflows.
    /// Includes triggers for both Informal and Formal LOD processes.
    /// </summary>
    public enum LineOfDutyTrigger
    {
        // ===== Common Triggers (used by both workflows) =====
        
        /// <summary>Initiates the LOD process.</summary>
        ProcessInitiated,
        
        /// <summary>Member reports a condition.</summary>
        ConditionReported,
        
        /// <summary>Adjudication is complete.</summary>
        AdjudicationComplete,
        
        /// <summary>Determination has been finalized.</summary>
        DeterminationFinalized,
        
        /// <summary>Appeal has been resolved.</summary>
        AppealResolved,

        // ===== Informal LOD Specific Triggers =====
        
        /// <summary>Initiation documentation is complete.</summary>
        InitiationComplete,
        
        /// <summary>Medical assessment is finished.</summary>
        AssessmentDone,
        
        /// <summary>Commander review is complete.</summary>
        ReviewFinished,
        
        /// <summary>Skip optional reviews and go to adjudication.</summary>
        SkipToAdjudication,
        
        /// <summary>Legal review is complete (Informal).</summary>
        LegalDone,
        
        /// <summary>Skip wing review after legal.</summary>
        SkipWingReview,
        
        /// <summary>Wing review is complete (Informal).</summary>
        WingDone,
        
        /// <summary>Notification complete without appeal.</summary>
        NotificationComplete,
        
        /// <summary>Member has filed an appeal (Informal).</summary>
        AppealFiled,

        // ===== Formal LOD Specific Triggers =====
        
        /// <summary>Questionable circumstances detected requiring formal investigation.</summary>
        QuestionableDetected,
        
        /// <summary>Investigating officer has been appointed.</summary>
        OfficerAppointed,
        
        /// <summary>Investigation is complete (DD Form 261).</summary>
        InvestigationComplete,
        
        /// <summary>Legal review is complete (Formal).</summary>
        LegalReviewComplete,
        
        /// <summary>Wing commander review is complete (Formal).</summary>
        WingReviewComplete,
        
        /// <summary>Member has filed an appeal (Formal).</summary>
        AppealRequested,
        
        /// <summary>No appeal was filed within the time limit.</summary>
        NoAppealRequested
    }
}
