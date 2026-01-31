using StateMachineExperiments.Modules.InformalLOD.Models;
using System;

namespace StateMachineExperiments.Modules.InformalLOD.Services
{
    /// <summary>
    /// Business rule service that determines review requirements and eligibility.
    /// Encapsulates business logic for legal review, wing review, and appeal decisions.
    /// </summary>
    public interface ILodBusinessRuleService
    {
        bool RequiresLegalReview(InformalLineOfDuty lodCase);
        bool RequiresWingReview(InformalLineOfDuty lodCase);
        bool IsAppealEligible(InformalLineOfDuty lodCase, DateTime appealDate);
        void ApplyBusinessRules(InformalLineOfDuty lodCase);
    }
}
