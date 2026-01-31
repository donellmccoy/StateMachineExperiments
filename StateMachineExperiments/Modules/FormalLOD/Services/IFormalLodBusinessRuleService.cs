using StateMachineExperiments.Modules.FormalLOD.Models;
using System;

namespace StateMachineExperiments.Modules.FormalLOD.Services
{
    /// <summary>
    /// Business rule service that determines investigation requirements and eligibility for Formal LOD.
    /// Encapsulates business logic for toxicology requirements, death case handling, and appeal decisions.
    /// </summary>
    public interface IFormalLodBusinessRuleService
    {
        /// <summary>
        /// Determines if toxicology reports are required for this case.
        /// </summary>
        bool RequiresToxicology(FormalLineOfDuty lodCase);
        
        /// <summary>
        /// Checks if investigation can proceed (toxicology complete if required).
        /// </summary>
        bool CanProceedFromInvestigation(FormalLineOfDuty lodCase);
        
        /// <summary>
        /// Determines if appeal is eligible based on time limits.
        /// Regular cases: 30 days; Death cases (next of kin): 180 days.
        /// </summary>
        bool IsAppealEligible(FormalLineOfDuty lodCase, DateTime appealDate);
        
        /// <summary>
        /// Applies all business rules to the case (toxicology requirements, etc.).
        /// </summary>
        void ApplyBusinessRules(FormalLineOfDuty lodCase);
    }
}
