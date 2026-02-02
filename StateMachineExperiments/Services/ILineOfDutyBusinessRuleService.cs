using StateMachineExperiments.Models;
using StateMachineExperiments.Enums;
using System;

namespace StateMachineExperiments.Services
{
    public interface ILineOfDutyBusinessRuleService
    {
        // Informal LOD rules
        bool RequiresLegalReview(LineOfDuty lodCase);
        bool RequiresWingReview(LineOfDuty lodCase);
        
        // Formal LOD rules
        bool RequiresToxicology(LineOfDuty lodCase);
        bool CanProceedFromInvestigation(LineOfDuty lodCase);
        
        // Common rules
        bool IsAppealEligible(LineOfDuty lodCase, DateTime appealDate);
        void ApplyBusinessRules(LineOfDuty lodCase);
    }
}
