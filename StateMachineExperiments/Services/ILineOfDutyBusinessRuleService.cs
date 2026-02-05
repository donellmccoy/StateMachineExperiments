using StateMachineExperiments.Models;
using StateMachineExperiments.Enums;
using System;

namespace StateMachineExperiments.Services
{
    public interface ILineOfDutyBusinessRuleService
    {
        // Informal LOD rules
        bool RequiresLegalReview(LineOfDutyCase lodCase);
        bool RequiresWingReview(LineOfDutyCase lodCase);
        
        // Formal LOD rules
        bool RequiresToxicology(LineOfDutyCase lodCase);
        bool CanProceedFromInvestigation(LineOfDutyCase lodCase);
        
        // Common rules
        bool IsAppealEligible(LineOfDutyCase lodCase, DateTime appealDate);
        void ApplyBusinessRules(LineOfDutyCase lodCase);
    }
}
