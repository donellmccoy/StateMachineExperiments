using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StateMachineExperiments.Common.Data;
using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.FormalLOD.Models;
using StateMachineExperiments.Modules.FormalLOD.Services;

namespace StateMachineExperiments.Modules.FormalLOD
{
    /// <summary>
    /// Module entry point for Formal Line of Duty demonstrations and workflows.
    /// Encapsulates setup and execution of formal LOD scenarios involving questionable circumstances.
    /// </summary>
    public class FormalLodModule
    {
        private readonly IFormalLodStateMachineService _service;
        private readonly IFormalLodBusinessRuleService _businessRules;
        private readonly IFormalLodDataService _dataService;

        public FormalLodModule(
            IFormalLodStateMachineService service,
            IFormalLodBusinessRuleService businessRules,
            IFormalLodDataService dataService)
        {
            _service = service;
            _businessRules = businessRules;
            _dataService = dataService;
        }

        /// <summary>
        /// Runs all demonstration scenarios for formal LOD processing.
        /// </summary>
        public async Task RunDemonstrationsAsync()
        {
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         FORMAL LINE OF DUTY DEMONSTRATIONS                ║");
            Console.WriteLine("║      (Questionable Circumstances - Full Investigation)    ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            await RunStandardFormalCaseWithToxicologyAsync();
            await RunExpeditedDeathCaseAsync();
            await RunFormalCaseWithAppealAsync();
            await RunPersistenceScenarioAsync();
        }

        private async Task RunStandardFormalCaseWithToxicologyAsync()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  SCENARIO 1: Standard Formal LOD with Toxicology Delay   ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var case1 = await _service.CreateNewCaseAsync("FORMAL-LOD-2026-001", "M456789", "MSgt Sarah Martinez", isDeathCase: false);
            
            // Set investigating officer and toxicology requirement
            case1.InvestigatingOfficerId = "O123456";
            case1.InvestigatingOfficerName = "Maj John Anderson";
            case1.ToxicologyRequired = true;
            case1.ToxicologyComplete = false;
            
            await _dataService.UpdateCaseAsync(case1);
            
            Console.WriteLine($"✓ Created Formal LOD Case: {case1.CaseNumber} (ID: {case1.Id})");
            Console.WriteLine($"  - Member: {case1.MemberName} ({case1.MemberId})");
            Console.WriteLine($"  - Investigating Officer: {case1.InvestigatingOfficerName}");
            Console.WriteLine($"  - Toxicology Required: {case1.ToxicologyRequired}");
            Console.WriteLine($"  - Death Case: {case1.IsDeathCase}\n");

            await _service.FireTriggerAsync(case1.Id, FormalLodTrigger.ProcessInitiated);
            await _service.FireTriggerAsync(case1.Id, FormalLodTrigger.ConditionReported, "Member reported injury under questionable circumstances");
            await _service.FireTriggerAsync(case1.Id, FormalLodTrigger.QuestionableDetected, "Formal investigation required");
            await _service.FireTriggerAsync(case1.Id, FormalLodTrigger.OfficerAppointed, $"Appointed {case1.InvestigatingOfficerName} as IO");
            
            Console.WriteLine("\n--- Investigation Phase (DD Form 261 - 50 day timeline) ---");
            
            // Attempt to complete investigation before toxicology results
            try
            {
                await _service.FireTriggerAsync(case1.Id, FormalLodTrigger.InvestigationComplete, "Investigation complete, awaiting toxicology");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Expected validation error: {ex.Message}");
            }
            
            // Simulate receiving toxicology reports
            Console.WriteLine("\n--- Toxicology Reports Received ---");
            var updatedCase1 = await _service.GetCaseAsync(case1.Id);
            updatedCase1!.ToxicologyComplete = true;
            await _dataService.UpdateCaseAsync(updatedCase1);
            
            // Now investigation can proceed
            await _service.FireTriggerAsync(case1.Id, FormalLodTrigger.InvestigationComplete, "Investigation complete with toxicology");
            await _service.FireTriggerAsync(case1.Id, FormalLodTrigger.LegalReviewComplete, "Legal review completed (45 days)");
            await _service.FireTriggerAsync(case1.Id, FormalLodTrigger.WingReviewComplete, "Wing Commander review completed (5 days)");
            await _service.FireTriggerAsync(case1.Id, FormalLodTrigger.AdjudicationComplete, "AFRC LOD Board adjudication complete (20 days)");
            
            updatedCase1 = await _service.GetCaseAsync(case1.Id);
            updatedCase1!.DeterminationResult = "In Line of Duty";
            await _dataService.UpdateCaseAsync(updatedCase1);
            
            await _service.FireTriggerAsync(case1.Id, FormalLodTrigger.DeterminationFinalized, "HQ AFRC/A1 approved determination");
            await _service.FireTriggerAsync(case1.Id, FormalLodTrigger.NoAppealRequested, "No appeal filed within 30 days");

            var finalCase1 = await _service.GetCaseAsync(case1.Id);
            Console.WriteLine($"\n✓ Final State: {finalCase1!.CurrentState}");
            Console.WriteLine($"✓ Total Transitions: {finalCase1.TransitionHistory.Count}");
            Console.WriteLine($"✓ Investigation Duration: {(finalCase1.InvestigationCompletionDate - finalCase1.InvestigationStartDate)?.Days} days\n");
        }

        private async Task RunExpeditedDeathCaseAsync()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  SCENARIO 2: Expedited Death Case (No Toxicology)        ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var case2 = await _service.CreateNewCaseAsync("FORMAL-LOD-2026-002", "M987654", "SrA David Lee", isDeathCase: true);
            
            case2.InvestigatingOfficerId = "O456789";
            case2.InvestigatingOfficerName = "Lt Col Jennifer Parker";
            case2.ToxicologyRequired = false;  // Not required for this death case scenario
            case2.ToxicologyComplete = true;
            
            await _dataService.UpdateCaseAsync(case2);
            
            Console.WriteLine($"✓ Created DEATH CASE: {case2.CaseNumber} (ID: {case2.Id})");
            Console.WriteLine($"  - Member: {case2.MemberName} ({case2.MemberId})");
            Console.WriteLine($"  - Investigating Officer: {case2.InvestigatingOfficerName}");
            Console.WriteLine($"  - EXPEDITED PROCESSING REQUIRED");
            Console.WriteLine($"  - Appeal Window: 180 days (next of kin)\n");

            await _service.FireTriggerAsync(case2.Id, FormalLodTrigger.ProcessInitiated);
            await _service.FireTriggerAsync(case2.Id, FormalLodTrigger.ConditionReported, "Death reported");
            await _service.FireTriggerAsync(case2.Id, FormalLodTrigger.QuestionableDetected, "Death case requires formal investigation");
            await _service.FireTriggerAsync(case2.Id, FormalLodTrigger.OfficerAppointed, $"Senior officer {case2.InvestigatingOfficerName} appointed");
            
            Console.WriteLine("\n--- EXPEDITED Investigation Phase ---");
            
            // Investigation proceeds immediately (no toxicology wait)
            await _service.FireTriggerAsync(case2.Id, FormalLodTrigger.InvestigationComplete, "Expedited investigation complete");
            await _service.FireTriggerAsync(case2.Id, FormalLodTrigger.LegalReviewComplete, "Expedited legal review");
            await _service.FireTriggerAsync(case2.Id, FormalLodTrigger.WingReviewComplete, "Expedited wing review");
            await _service.FireTriggerAsync(case2.Id, FormalLodTrigger.AdjudicationComplete, "Expedited board review");
            
            var updatedCase2 = await _service.GetCaseAsync(case2.Id);
            updatedCase2!.DeterminationResult = "In Line of Duty";
            await _dataService.UpdateCaseAsync(updatedCase2);
            
            await _service.FireTriggerAsync(case2.Id, FormalLodTrigger.DeterminationFinalized, "Expedited determination");
            await _service.FireTriggerAsync(case2.Id, FormalLodTrigger.NoAppealRequested, "No appeal from next of kin (180-day window)");

            var finalCase2 = await _service.GetCaseAsync(case2.Id);
            Console.WriteLine($"\n✓ Final State: {finalCase2!.CurrentState}");
            Console.WriteLine($"✓ Total Transitions: {finalCase2.TransitionHistory.Count}");
            Console.WriteLine($"✓ EXPEDITED Processing Time: {(finalCase2.InvestigationCompletionDate - finalCase2.InvestigationStartDate)?.Days} days\n");
        }

        private async Task RunFormalCaseWithAppealAsync()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  SCENARIO 3: Formal LOD with Appeal                      ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var case3 = await _service.CreateNewCaseAsync("FORMAL-LOD-2026-003", "M234567", "Capt Emily Chen", isDeathCase: false);
            
            case3.InvestigatingOfficerId = "O789012";
            case3.InvestigatingOfficerName = "Maj Robert Wilson";
            case3.ToxicologyRequired = false;
            case3.ToxicologyComplete = true;
            
            await _dataService.UpdateCaseAsync(case3);
            
            Console.WriteLine($"✓ Created Case: {case3.CaseNumber} (ID: {case3.Id})");
            Console.WriteLine($"  - Member: {case3.MemberName} ({case3.MemberId})\n");

            await _service.FireTriggerAsync(case3.Id, FormalLodTrigger.ProcessInitiated);
            await _service.FireTriggerAsync(case3.Id, FormalLodTrigger.ConditionReported);
            await _service.FireTriggerAsync(case3.Id, FormalLodTrigger.QuestionableDetected);
            await _service.FireTriggerAsync(case3.Id, FormalLodTrigger.OfficerAppointed);
            await _service.FireTriggerAsync(case3.Id, FormalLodTrigger.InvestigationComplete);
            await _service.FireTriggerAsync(case3.Id, FormalLodTrigger.LegalReviewComplete);
            await _service.FireTriggerAsync(case3.Id, FormalLodTrigger.WingReviewComplete);
            await _service.FireTriggerAsync(case3.Id, FormalLodTrigger.AdjudicationComplete);
            
            var updatedCase3 = await _service.GetCaseAsync(case3.Id);
            updatedCase3!.DeterminationResult = "Not In Line of Duty";
            await _dataService.UpdateCaseAsync(updatedCase3);
            
            await _service.FireTriggerAsync(case3.Id, FormalLodTrigger.DeterminationFinalized, "Determination: Not In LOD");
            await _service.FireTriggerAsync(case3.Id, FormalLodTrigger.AppealRequested, "Member filed appeal within 30 days");
            await _service.FireTriggerAsync(case3.Id, FormalLodTrigger.AppealResolved, "HQ AFRC/CD reviewed appeal - original determination upheld");

            var finalCase3 = await _service.GetCaseAsync(case3.Id);
            Console.WriteLine($"\n✓ Final State: {finalCase3!.CurrentState}");
            Console.WriteLine($"✓ Appeal Filed: {finalCase3.AppealFiled}");
            Console.WriteLine($"✓ Total Transitions: {finalCase3.TransitionHistory.Count}\n");
        }

        private async Task RunPersistenceScenarioAsync()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  SCENARIO 4: Persistence & Resumability                  ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var case4 = await _service.CreateNewCaseAsync("FORMAL-LOD-2026-004", "M567890", "TSgt Michael Brown");
            
            case4.InvestigatingOfficerId = "O345678";
            case4.InvestigatingOfficerName = "Capt Lisa Rodriguez";
            await _dataService.UpdateCaseAsync(case4);
            
            Console.WriteLine($"✓ Created Case: {case4.CaseNumber}\n");

            await _service.FireTriggerAsync(case4.Id, FormalLodTrigger.ProcessInitiated);
            await _service.FireTriggerAsync(case4.Id, FormalLodTrigger.ConditionReported);
            await _service.FireTriggerAsync(case4.Id, FormalLodTrigger.QuestionableDetected);
            
            Console.WriteLine($"✓ Case paused at state: {(await _service.GetCaseAsync(case4.Id))!.CurrentState}");
            Console.WriteLine("  (Simulating system restart during investigation phase...)\n");

            var loadedCase = await _service.GetCaseByCaseNumberAsync("FORMAL-LOD-2026-004");
            Console.WriteLine($"✓ Case loaded from database: {loadedCase!.CaseNumber}");
            Console.WriteLine($"✓ Current State: {loadedCase.CurrentState}");
            Console.WriteLine($"✓ Investigating Officer: {loadedCase.InvestigatingOfficerName}");
            
            var permittedTriggers = await _service.GetPermittedTriggersAsync(loadedCase.Id);
            Console.WriteLine($"✓ Permitted Triggers: {string.Join(", ", permittedTriggers)}\n");

            await _service.FireTriggerAsync(loadedCase.Id, FormalLodTrigger.OfficerAppointed);
            Console.WriteLine($"✓ Resumed and advanced to: {(await _service.GetCaseAsync(loadedCase.Id))!.CurrentState}\n");
        }
    }
}
