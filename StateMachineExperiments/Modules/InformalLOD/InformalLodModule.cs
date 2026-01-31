using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StateMachineExperiments.Common.Data;
using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.InformalLOD.Models;
using StateMachineExperiments.Modules.InformalLOD.Services;

namespace StateMachineExperiments.Modules.InformalLOD
{
    /// <summary>
    /// Module entry point for Informal Line of Duty demonstrations and workflows.
    /// Encapsulates setup and execution of informal LOD scenarios.
    /// </summary>
    public class InformalLodModule
    {
        private readonly ILodStateMachineService _service;
        private readonly ILodBusinessRuleService _businessRules;
        private readonly ILodDataService _dataService;
        private readonly ILodVisualizationService _visualization;
        private readonly IEventPublisher _eventPublisher;

        public InformalLodModule(
            ILodStateMachineService service,
            ILodBusinessRuleService businessRules,
            ILodDataService dataService,
            ILodVisualizationService visualization,
            IEventPublisher eventPublisher)
        {
            _service = service;
            _businessRules = businessRules;
            _dataService = dataService;
            _visualization = visualization;
            _eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Runs all demonstration scenarios for informal LOD processing.
        /// </summary>
        public async Task RunDemonstrationsAsync()
        {
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         INFORMAL LINE OF DUTY DEMONSTRATIONS              ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            await RunComplexCaseScenarioAsync();
            await RunFastTrackScenarioAsync();
            await RunPersistenceScenarioAsync();
            await RunValidationDemoAsync();
            await RunVisualizationDemoAsync();
            await ShowEventSummaryAsync();
        }

        private async Task RunComplexCaseScenarioAsync()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  SCENARIO 1: Case with Optional Reviews & Appeal         ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var case1 = await _service.CreateNewCaseAsync("LOD-2026-001", "M123456", "SSgt John Doe");
            
            // Set case details - business rules will automatically determine required reviews
            case1.InjurySeverity = 8;
            case1.EstimatedCost = 75000;
            
            // Apply business rules automatically
            _businessRules.ApplyBusinessRules(case1);
            await _dataService.UpdateCaseAsync(case1);
            
            Console.WriteLine($"✓ Created Case: {case1.CaseNumber} (ID: {case1.Id})");
            Console.WriteLine($"  - Injury Severity: {case1.InjurySeverity}");
            Console.WriteLine($"  - Estimated Cost: ${case1.EstimatedCost:N0}");
            Console.WriteLine($"  - Requires Legal Review: {case1.RequiresLegalReview} (auto-determined)");
            Console.WriteLine($"  - Requires Wing Review: {case1.RequiresWingReview} (auto-determined)\n");

            await _service.FireTriggerAsync(case1.Id, LodTrigger.ProcessInitiated);
            await _service.FireTriggerAsync(case1.Id, LodTrigger.ConditionReported, notes: "Member reported injury during PT");
            await _service.FireTriggerAsync(case1.Id, LodTrigger.InitiationComplete, "All medical records received");
            await _service.FireTriggerAsync(case1.Id, LodTrigger.AssessmentDone, notes: "Medical assessment completed");
            await _service.FireTriggerAsync(case1.Id, LodTrigger.ReviewFinished, "Routing to legal review");
            await _service.FireTriggerAsync(case1.Id, LodTrigger.LegalDone, "Routing to wing review");
            await _service.FireTriggerAsync(case1.Id, LodTrigger.WingDone, notes: "Wing commander approved");
            await _service.FireTriggerAsync(case1.Id, LodTrigger.AdjudicationComplete, notes: "Board determined in line of duty");
            await _service.FireTriggerAsync(case1.Id, LodTrigger.DeterminationFinalized, notes: "Approved by HQ AFRC/A1");
            await _service.FireTriggerAsync(case1.Id, LodTrigger.AppealFiled, "Member filed appeal within deadline");
            await _service.FireTriggerAsync(case1.Id, LodTrigger.AppealResolved, notes: "Appeal denied, original determination upheld");

            var finalCase1 = await _service.GetCaseAsync(case1.Id);
            Console.WriteLine($"\n✓ Final State: {finalCase1!.CurrentState}");
            Console.WriteLine($"✓ Total Transitions: {finalCase1.TransitionHistory.Count}\n");

            // Show transition history
            Console.WriteLine("Transition History:");
            var history = await _service.GetCaseHistoryAsync(case1.Id);
            foreach (var transition in history.Take(3))
            {
                Console.WriteLine($"  [{transition.Timestamp:HH:mm:ss}] {transition.FromState} → {transition.ToState}");
            }
            Console.WriteLine($"  ... ({history.Count - 3} more transitions)\n");
        }

        private async Task RunFastTrackScenarioAsync()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  SCENARIO 2: Fast-Track Case (No Optional Reviews)       ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var case2 = await _service.CreateNewCaseAsync("LOD-2026-002", "M789012", "A1C Jane Smith");
            
            case2.InjurySeverity = 3;
            case2.EstimatedCost = 5000;
            
            _businessRules.ApplyBusinessRules(case2);
            await _dataService.UpdateCaseAsync(case2);
            
            Console.WriteLine($"✓ Created Case: {case2.CaseNumber} (ID: {case2.Id})");
            Console.WriteLine($"  - Injury Severity: {case2.InjurySeverity}");
            Console.WriteLine($"  - Estimated Cost: ${case2.EstimatedCost:N0}");
            Console.WriteLine($"  - Requires Legal Review: {case2.RequiresLegalReview}");
            Console.WriteLine($"  - Requires Wing Review: {case2.RequiresWingReview}\n");

            await _service.FireTriggerAsync(case2.Id, LodTrigger.ProcessInitiated);
            await _service.FireTriggerAsync(case2.Id, LodTrigger.ConditionReported);
            await _service.FireTriggerAsync(case2.Id, LodTrigger.InitiationComplete);
            await _service.FireTriggerAsync(case2.Id, LodTrigger.AssessmentDone);
            await _service.FireTriggerAsync(case2.Id, LodTrigger.SkipToAdjudication, "No optional reviews needed");
            await _service.FireTriggerAsync(case2.Id, LodTrigger.AdjudicationComplete);
            await _service.FireTriggerAsync(case2.Id, LodTrigger.DeterminationFinalized);
            await _service.FireTriggerAsync(case2.Id, LodTrigger.NotificationComplete, "No appeal filed");

            var finalCase2 = await _service.GetCaseAsync(case2.Id);
            Console.WriteLine($"\n✓ Final State: {finalCase2!.CurrentState}");
            Console.WriteLine($"✓ Total Transitions: {finalCase2.TransitionHistory.Count}\n");
        }

        private async Task RunPersistenceScenarioAsync()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  SCENARIO 3: Persistence & Resumability Demo             ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var case3 = await _service.CreateNewCaseAsync("LOD-2026-003", "M345678", "TSgt Robert Johnson");
            Console.WriteLine($"✓ Created Case: {case3.CaseNumber}\n");

            await _service.FireTriggerAsync(case3.Id, LodTrigger.ProcessInitiated);
            await _service.FireTriggerAsync(case3.Id, LodTrigger.ConditionReported);
            
            Console.WriteLine($"✓ Case paused at state: {(await _service.GetCaseAsync(case3.Id))!.CurrentState}");
            Console.WriteLine("  (Simulating system restart...)\n");

            var loadedCase = await _service.GetCaseByCaseNumberAsync("LOD-2026-003");
            Console.WriteLine($"✓ Case loaded from database: {loadedCase!.CaseNumber}");
            Console.WriteLine($"✓ Current State: {loadedCase.CurrentState}");
            
            var permittedTriggers = await _service.GetPermittedTriggersAsync(loadedCase.Id);
            Console.WriteLine($"✓ Permitted Triggers: {string.Join(", ", permittedTriggers)}\n");

            await _service.FireTriggerAsync(loadedCase.Id, LodTrigger.InitiationComplete);
            Console.WriteLine($"✓ Resumed and advanced to: {(await _service.GetCaseAsync(loadedCase.Id))!.CurrentState}\n");
        }

        private async Task RunValidationDemoAsync()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  SCENARIO 4: Validation & Error Handling                 ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var case4 = await _service.CreateNewCaseAsync("LOD-2026-004", memberName: "Capt Mike Wilson");
            Console.WriteLine($"✓ Created Case: {case4.CaseNumber} (missing Member ID)");
            
            var validation = await _service.ValidateTransitionAsync(case4.Id, LodTrigger.ProcessInitiated);
            if (!validation.IsValid)
            {
                Console.WriteLine($"⚠ Validation would fail: {string.Join(", ", validation.Errors)}");
            }
            
            await _service.FireTriggerAsync(case4.Id, LodTrigger.ProcessInitiated);
            
            try
            {
                await _service.FireTriggerAsync(case4.Id, LodTrigger.ConditionReported);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✓ Caught expected validation error: {ex.Message}\n");
            }
        }

        private async Task RunVisualizationDemoAsync()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  SCENARIO 5: State Machine Visualization                 ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var case1 = await _service.GetCaseByCaseNumberAsync("LOD-2026-001");
            if (case1 != null)
            {
                var dotGraph = _visualization.GenerateDotGraph(case1);
                _visualization.SaveDotGraphToFile(case1, "informal_lod_state_machine.dot");
                Console.WriteLine("✓ State machine diagram saved to: informal_lod_state_machine.dot");
                Console.WriteLine("  (Use Graphviz: dot -Tpng informal_lod_state_machine.dot -o diagram.png)\n");
            }
        }

        private async Task ShowEventSummaryAsync()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  DEMONSTRATION: Published Domain Events                  ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            if (_eventPublisher is InMemoryEventPublisher inMemoryPublisher)
            {
                var events = inMemoryPublisher.GetPublishedEvents();
                Console.WriteLine($"✓ Total events published: {events.Count}");
                Console.WriteLine($"  - Case Created Events: {events.Count(e => e.GetType().Name.Contains("Created"))}");
                Console.WriteLine($"  - State Changed Events: {events.Count(e => e.GetType().Name.Contains("StateChanged"))}");
                Console.WriteLine($"  - Determination Events: {events.Count(e => e.GetType().Name.Contains("Determination"))}");
                Console.WriteLine($"  - Appeal Events: {events.Count(e => e.GetType().Name.Contains("Appeal"))}\n");
            }
        }
    }
}
