namespace StateMachineExperiments.Enums
{
    /// <summary>
    /// Represents the authority levels involved in LOD processing.
    /// Based on DAFI 36-2910 for AFRC LOD (both Informal and Formal).
    /// </summary>
    public enum LineOfDutyAuthority
    {
        None,
        Member,                 // The reporting member
        LodMfp,                 // LOD-MFP (e.g., Medical Force Provider)
        MedicalProvider,        // Military Medical Provider
        ImmediateCommander,     // First full-time CC or Detachment CC (Informal LOD)
        AppointingAuthority,    // Senior AFR CC or HQ RIO/CC (Formal LOD - appoints investigating officer)
        InvestigatingOfficer,   // Appointed O-3+ officer (Formal LOD)
        LegalAdvisor,           // Wing Staff Judge Advocate (optional in Informal, required in Formal)
        WingCommander,          // Wing CC or equivalent (optional)
        ReviewingBoard,         // AFRC LOD Determination Board (SG, JA, A1)
        ApprovingAuthority,     // HQ AFRC/A1
        LodPm,                  // LOD Program Manager (for notifications)
        AppellateAuthority      // HQ AFRC/CD (for appeals)
    }
}
