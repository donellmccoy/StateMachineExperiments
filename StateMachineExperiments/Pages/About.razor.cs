using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace StateMachineExperiments.Pages
{
    public partial class About : ComponentBase
    {
        private readonly List<DevelopmentPractice> developmentPractices = new()
        {
            new DevelopmentPractice
            {
                Icon = "layers",
                Title = "Clean Architecture",
                Description = "Separated concerns with modular design and clear boundaries"
            },
            new DevelopmentPractice
            {
                Icon = "call_split",
                Title = "Dependency Injection",
                Description = "All services use DI for testability and maintainability"
            },
            new DevelopmentPractice
            {
                Icon = "sync",
                Title = "Async/Await Patterns",
                Description = "Non-blocking async operations throughout the application"
            },
            new DevelopmentPractice
            {
                Icon = "storage",
                Title = "Entity Framework Core",
                Description = "DbContextFactory pattern for proper lifecycle management"
            },
            new DevelopmentPractice
            {
                Icon = "task_alt",
                Title = "Comprehensive Testing",
                Description = "Unit tests for business logic and state machine transitions"
            },
            new DevelopmentPractice
            {
                Icon = "security",
                Title = "Type Safety",
                Description = "Enum-based states, triggers, and authorities for compile-time safety"
            }
        };
    }

    public class DevelopmentPractice
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
