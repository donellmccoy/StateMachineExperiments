namespace StateMachineExperiments.Enums
{
    /// <summary>
    /// Represents the possible states in Line of Duty workflows (both Informal and Formal).
    /// Based on DAFI 36-2910 for AFRC LOD processing.
    /// </summary>
    public enum LineOfDutyState
    {
        // Common states
        /// <summary>Initial state before processing begins.</summary>
        Start,
        /// <summary>Member has reported a condition.</summary>
        MemberReports,
        /// <summary>Board adjudication in progress.</summary>
        BoardAdjudication,
        /// <summary>Final determination has been made.</summary>
        Determination,
        /// <summary>Member is being notified of the determination.</summary>
        Notification,
        /// <summary>Appeal has been filed and is being processed.</summary>
        Appeal,
        /// <summary>Final state - case is closed.</summary>
        End,

        // Informal LOD specific states
        /// <summary>LOD initiation process is underway (Informal).</summary>
        LodInitiation,
        /// <summary>Medical assessment is in progress (Informal).</summary>
        MedicalAssessment,
        /// <summary>Commander is reviewing the case (Informal).</summary>
        CommanderReview,
        /// <summary>Optional legal review state (Informal).</summary>
        OptionalLegal,
        /// <summary>Optional wing commander review state (Informal).</summary>
        OptionalWing,

        // Formal LOD specific states
        /// <summary>Formal LOD process has been initiated due to questionable circumstances (Formal).</summary>
        FormalInitiation,
        /// <summary>Appointing authority is assigning an investigating officer (Formal).</summary>
        AppointingOfficer,
        /// <summary>Investigating officer is conducting the investigation (Formal).</summary>
        Investigation,
        /// <summary>Wing legal review is in progress (Formal).</summary>
        WingLegalReview,
        /// <summary>Wing commander review is in progress (Formal).</summary>
        WingCommanderReview
    }
}
