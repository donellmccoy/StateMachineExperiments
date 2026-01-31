using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StateMachineExperiments.Common.Data;
using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.InformalLOD;
using StateMachineExperiments.Modules.InformalLOD.Services;

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
            ILodDataService dataService = new LodDataService(context);
            ILodBusinessRuleService businessRules = new LodBusinessRuleService();
            IEventPublisher eventPublisher = new InMemoryEventPublisher();
            ILodStateMachineFactory stateMachineFactory = new LodStateMachineFactory();
            ILodTransitionValidator validator = new LodTransitionValidator(businessRules);
            ILodVisualizationService visualization = new LodVisualizationService(stateMachineFactory);
            
            var service = new LodStateMachineService(
                dataService, 
                businessRules, 
                validator, 
                stateMachineFactory, 
                eventPublisher);

            // Create and run module
            var informalModule = new InformalLodModule(
                service,
                businessRules,
                dataService,
                visualization,
                eventPublisher);

            await informalModule.RunDemonstrationsAsync();
        }

        private static async Task ShowDatabaseSummaryAsync(LodDbContext context)
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  Database Summary                                         ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var totalCases = await context.LodCases.CountAsync();
            var totalTransitions = await context.TransitionHistory.CountAsync();
            var casesInProgress = await context.LodCases.CountAsync(c => c.CurrentState != "End");

            Console.WriteLine($"  Total Cases: {totalCases}");
            Console.WriteLine($"  Total Transitions: {totalTransitions}");
            Console.WriteLine($"  Cases In Progress: {casesInProgress}");
            Console.WriteLine($"  Cases Completed: {totalCases - casesInProgress}\n");
        }
    }
}
