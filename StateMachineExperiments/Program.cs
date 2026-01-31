using Microsoft.EntityFrameworkCore;
using StateMachineExperiments.Data;
using StateMachineExperiments.Models;
using StateMachineExperiments.Services;

namespace StateMachineExperiments
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== LOD State Machine with Stateless & Entity Framework ===\n");

            // Setup database
            var optionsBuilder = new DbContextOptionsBuilder<LodDbContext>();
            optionsBuilder.UseSqlite("Data Source=lod_cases.db");

            using var context = new LodDbContext(optionsBuilder.Options);
            
            // Ensure database is created
            await context.Database.EnsureDeletedAsync(); // Clean slate for demo
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("✓ Database initialized\n");

            // Setup dependency injection
            ILodDataService dataService = new LodDataService(context);
            var service = new LodStateMachineService(dataService);

            // ===========================================
            // SCENARIO 1: Full workflow with all optional reviews and appeal
            // ===========================================
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  SCENARIO 1: Case with Optional Reviews & Appeal         ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var case1 = await service.CreateNewCaseAsync("LOD-2026-001", "M123456", "SSgt John Doe");
            
            // Set business decision flags - this case requires full review process
            case1.RequiresLegalReview = true;
            case1.RequiresWingReview = true;
            case1.InjurySeverity = 8;
            case1.EstimatedCost = 75000;
            await dataService.UpdateCaseAsync(case1);
            
            Console.WriteLine($"✓ Created Case: {case1.CaseNumber} (ID: {case1.Id})");
            Console.WriteLine($"  - Requires Legal Review: {case1.RequiresLegalReview}");
            Console.WriteLine($"  - Requires Wing Review: {case1.RequiresWingReview}\n");

            await service.FireTriggerAsync(case1.Id, LodTrigger.ProcessInitiated);
            await service.FireTriggerAsync(case1.Id, LodTrigger.ConditionReported, notes: "Member reported injury during PT");
            await service.FireTriggerAsync(case1.Id, LodTrigger.InitiationComplete, "All medical records received");
            await service.FireTriggerAsync(case1.Id, LodTrigger.AssessmentDone, notes: "Medical assessment completed");
            await service.FireTriggerAsync(case1.Id, LodTrigger.ReviewFinished, "Routing to legal review");
            await service.FireTriggerAsync(case1.Id, LodTrigger.LegalDone, "Routing to wing review");
            await service.FireTriggerAsync(case1.Id, LodTrigger.WingDone, notes: "Wing commander approved");
            await service.FireTriggerAsync(case1.Id, LodTrigger.AdjudicationComplete, notes: "Board determined in line of duty");
            await service.FireTriggerAsync(case1.Id, LodTrigger.DeterminationFinalized, notes: "Approved by HQ AFRC/A1");
            await service.FireTriggerAsync(case1.Id, LodTrigger.AppealFiled, "Member filed appeal within deadline");
            await service.FireTriggerAsync(case1.Id, LodTrigger.AppealResolved, notes: "Appeal denied, original determination upheld");

            var finalCase1 = await service.GetCaseAsync(case1.Id);
            Console.WriteLine($"\n✓ Final State: {finalCase1!.CurrentState}");
            Console.WriteLine($"✓ Total Transitions: {finalCase1.TransitionHistory.Count}\n");

            // ===========================================
            // SCENARIO 2: Fast-track without optional reviews
            // ===========================================
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  SCENARIO 2: Fast-Track Case (No Optional Reviews)       ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var case2 = await service.CreateNewCaseAsync("LOD-2026-002", "M789012", "A1C Jane Smith");
            
            // Simple case - no optional reviews needed
            case2.RequiresLegalReview = false;
            case2.RequiresWingReview = false;
            case2.InjurySeverity = 3;
            case2.EstimatedCost = 5000;
            await dataService.UpdateCaseAsync(case2);
            
            Console.WriteLine($"✓ Created Case: {case2.CaseNumber} (ID: {case2.Id})");
            Console.WriteLine($"  - Requires Legal Review: {case2.RequiresLegalReview}");
            Console.WriteLine($"  - Requires Wing Review: {case2.RequiresWingReview}\n");

            await service.FireTriggerAsync(case2.Id, LodTrigger.ProcessInitiated);
            await service.FireTriggerAsync(case2.Id, LodTrigger.ConditionReported);
            await service.FireTriggerAsync(case2.Id, LodTrigger.InitiationComplete);
            await service.FireTriggerAsync(case2.Id, LodTrigger.AssessmentDone);
            await service.FireTriggerAsync(case2.Id, LodTrigger.SkipToAdjudication, "No optional reviews needed - fast track");
            await service.FireTriggerAsync(case2.Id, LodTrigger.AdjudicationComplete);
            await service.FireTriggerAsync(case2.Id, LodTrigger.DeterminationFinalized);
            await service.FireTriggerAsync(case2.Id, LodTrigger.NotificationComplete, "No appeal filed - case closed");

            var finalCase2 = await service.GetCaseAsync(case2.Id);
            Console.WriteLine($"\n✓ Final State: {finalCase2!.CurrentState}");
            Console.WriteLine($"✓ Total Transitions: {finalCase2.TransitionHistory.Count}\n");

            // ===========================================
            // Display Transition History
            // ===========================================
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  CASE 1: Complete Transition History                     ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var history1 = await service.GetCaseHistoryAsync(case1.Id);
            foreach (var transition in history1)
            {
                Console.WriteLine($"  [{transition.Timestamp:HH:mm:ss}] {transition.FromState,-20} → {transition.ToState,-20}");
                Console.WriteLine($"                Trigger: {transition.Trigger}");
                Console.WriteLine($"                Authority: {transition.PerformedByAuthority}");
                if (!string.IsNullOrEmpty(transition.Notes))
                {
                    Console.WriteLine($"                Notes: {transition.Notes}");
                }

                Console.WriteLine();
            }

            // ===========================================
            // Demonstrate Persistence & Resumability
            // ===========================================
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  SCENARIO 3: Persistence & Resumability Demo             ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var case3 = await service.CreateNewCaseAsync("LOD-2026-003", "M345678", "TSgt Robert Johnson");
            Console.WriteLine($"✓ Created Case: {case3.CaseNumber}\n");

            await service.FireTriggerAsync(case3.Id, LodTrigger.ProcessInitiated);
            await service.FireTriggerAsync(case3.Id, LodTrigger.ConditionReported);
            
            Console.WriteLine($"✓ Case paused at state: {(await service.GetCaseAsync(case3.Id))!.CurrentState}");
            Console.WriteLine("  (Simulating system restart...)\n");

            // Simulate loading from database after restart
            var loadedCase = await service.GetCaseByCaseNumberAsync("LOD-2026-003");
            Console.WriteLine($"✓ Case loaded from database: {loadedCase!.CaseNumber}");
            Console.WriteLine($"✓ Current State: {loadedCase.CurrentState}");
            
            var permittedTriggers = await service.GetPermittedTriggersAsync(loadedCase.Id);
            Console.WriteLine($"✓ Permitted Triggers: {string.Join(", ", permittedTriggers)}\n");

            // Continue workflow
            await service.FireTriggerAsync(loadedCase.Id, LodTrigger.InitiationComplete);
            Console.WriteLine($"✓ Resumed and advanced to: {(await service.GetCaseAsync(loadedCase.Id))!.CurrentState}\n");

            // ===========================================
            // Summary Statistics
            // ===========================================
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  Database Summary                                         ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var totalCases = await context.LodCases.CountAsync();
            var totalTransitions = await context.TransitionHistory.CountAsync();
            var casesInProgress = await context.LodCases.CountAsync(c => c.CurrentState != nameof(LodState.End));

            Console.WriteLine($"  Total Cases: {totalCases}");
            Console.WriteLine($"  Total Transitions: {totalTransitions}");
            Console.WriteLine($"  Cases In Progress: {casesInProgress}");
            Console.WriteLine($"  Cases Completed: {totalCases - casesInProgress}\n");

            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  Demo Complete - Check 'lod_cases.db' for persisted data ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        }
    }
}