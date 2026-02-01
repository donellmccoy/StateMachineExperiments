using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StateMachineExperiments.Common.Data;
using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.InformalLOD;
using StateMachineExperiments.Modules.InformalLOD.Services;
using StateMachineExperiments.Modules.FormalLOD;
using StateMachineExperiments.Modules.FormalLOD.Services;

namespace StateMachineExperiments
{
    /// <summary>
    /// Main entry point demonstrating modular LOD state machine architecture.
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║     State Machine Experiments - Modular Architecture     ║");
            Console.WriteLine("║     Line of Duty (LOD) Case Management System            ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            // Setup database
            var optionsBuilder = new DbContextOptionsBuilder<LodDbContext>();
            optionsBuilder.UseSqlite("Data Source=lod_cases.db");

            using var context = new LodDbContext(optionsBuilder.Options);
            
            // Ensure database is created
            await context.Database.EnsureDeletedAsync(); // Clean slate for demo
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("✓ Database initialized\n");

            // ===========================================
            // Run Informal LOD Module Demonstrations
            // ===========================================
            await RunInformalLodDemosAsync(context);

            // ===========================================
            // Run Formal LOD Module Demonstrations
            // ===========================================
            await RunFormalLodDemosAsync(context);

            // ===========================================
            // Show Database Summary
            // ===========================================
            await ShowDatabaseSummaryAsync(context);

            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  Demo Complete - Check 'lod_cases.db' for persisted data ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        }

        private static async Task RunInformalLodDemosAsync(LodDbContext context)
        {
            // Setup dependency injection for Informal LOD module
            INotificationService notificationService = new NotificationService();
            ILodDataService dataService = new LodDataService(context);
            ILodBusinessRuleService businessRules = new LodBusinessRuleService();
            ILodStateMachineFactory stateMachineFactory = new LodStateMachineFactory(notificationService);
            ILodTransitionValidator validator = new LodTransitionValidator(businessRules);
            ILodVisualizationService visualization = new LodVisualizationService(stateMachineFactory);
            
            var service = new LodStateMachineService(
                dataService, 
                businessRules, 
                validator, 
                stateMachineFactory, 
                notificationService);

            // Create and run module
            var informalModule = new InformalLodModule(
                service,
                businessRules,
                dataService,
                visualization);

            await informalModule.RunDemonstrationsAsync();
        }

        private static async Task RunFormalLodDemosAsync(LodDbContext context)
        {
            // Setup dependency injection for Formal LOD module
            INotificationService notificationService = new NotificationService();
            IFormalLodDataService dataService = new FormalLodDataService(context);
            IFormalLodBusinessRuleService businessRules = new FormalLodBusinessRuleService();
            IFormalLodStateMachineFactory stateMachineFactory = new FormalLodStateMachineFactory(notificationService);
            IFormalLodTransitionValidator validator = new FormalLodTransitionValidator(businessRules);
            
            var service = new FormalLodStateMachineService(
                dataService, 
                businessRules, 
                validator, 
                stateMachineFactory, 
                notificationService);

            // Create and run module
            var formalModule = new FormalLodModule(
                service,
                businessRules,
                dataService);

            await formalModule.RunDemonstrationsAsync();
        }

        private static async Task ShowDatabaseSummaryAsync(LodDbContext context)
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  Database Summary                                         ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var totalInformalCases = await context.LodCases.CountAsync();
            var totalInformalTransitions = await context.TransitionHistory.CountAsync();
            var informalCasesInProgress = await context.LodCases.CountAsync(c => c.CurrentState != "End");

            var totalFormalCases = await context.FormalLodCases.CountAsync();
            var totalFormalTransitions = await context.FormalTransitionHistory.CountAsync();
            var formalCasesInProgress = await context.FormalLodCases.CountAsync(c => c.CurrentState != "End");

            Console.WriteLine("  Informal LOD Cases:");
            Console.WriteLine($"    Total Cases: {totalInformalCases}");
            Console.WriteLine($"    Total Transitions: {totalInformalTransitions}");
            Console.WriteLine($"    Cases In Progress: {informalCasesInProgress}");
            Console.WriteLine($"    Cases Completed: {totalInformalCases - informalCasesInProgress}\n");

            Console.WriteLine("  Formal LOD Cases:");
            Console.WriteLine($"    Total Cases: {totalFormalCases}");
            Console.WriteLine($"    Total Transitions: {totalFormalTransitions}");
            Console.WriteLine($"    Cases In Progress: {formalCasesInProgress}");
            Console.WriteLine($"    Cases Completed: {totalFormalCases - formalCasesInProgress}\n");

            Console.WriteLine($"  Grand Total Cases: {totalInformalCases + totalFormalCases}");
            Console.WriteLine($"  Grand Total Transitions: {totalInformalTransitions + totalFormalTransitions}\n");
        }
    }
}
