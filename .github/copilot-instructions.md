# GitHub Copilot Instructions - State Machine Experiments

## Project Overview

This is a **Line of Duty (LOD) Case Management System** for Air Force Reserve Command (AFRC) built with:

- **Blazor WebAssembly** (client-side SPA)
- **Stateless 5.20.0** (hierarchical state machine library)
- **Entity Framework Core 10.0.2** (with in-memory database)
- **Radzen Blazor** (UI component library with Material 3 theme)
- **.NET 10.0** and **C# 12**

The system demonstrates advanced state machine patterns for managing complex workflows with:
- Multiple state transitions and conditional branching
- Persistent state storage and transition history tracking
- Event-driven architecture with domain events
- Modular organization for different LOD processes

## Architecture

### Modular Structure

```
Common/
├── Data/                    # Shared DbContext (LodDbContext)
├── Exceptions/              # Custom exception types
└── Infrastructure/          # Base classes (BaseEvent, INotificationService, etc.)

Modules/
├── InformalLOD/            # Informal LOD process (IMPLEMENTED)
│   ├── Models/              # InformalLineOfDuty, LodState, LodTrigger enums
│   ├── Events/              # Domain events (StateChangedEvent, etc.)
│   ├── Services/            # Business logic and state machine services
│   └── Tests/               # Unit tests with xUnit
└── FormalLOD/              # Formal LOD process (IN PROGRESS)
    ├── Models/              # FormalLineOfDuty, FormalLodState, FormalLodTrigger enums
    ├── Events/              # Domain events
    ├── Services/            # Business logic and state machine services
    └── Tests/               # Unit tests
```

### Key Design Patterns

1. **State Machine Pattern** - Using Stateless library for workflow management
2. **Factory Pattern** - `LodStateMachineFactory` / `FormalLodStateMachineFactory` for creating configured state machines
3. **Service Layer Pattern** - Separation of concerns with dedicated services
4. **Domain Events** - Event-driven architecture for state changes
5. **Repository Pattern** - Data access through `LodDataService` / `FormalLodDataService`

## Coding Guidelines

### State Machine Implementation

When creating or modifying state machines:

```csharp
// ✅ DO: Use strongly-typed enums for states and triggers
public enum LodState { Start, MemberReports, LodInitiation, /* ... */ }
public enum LodTrigger { ProcessInitiated, ConditionReported, /* ... */ }

// ✅ DO: Configure state machine with entry/exit handlers
stateMachine.Configure(LodState.MemberReports)
    .OnEntryAsync(async () => await OnStateEntryAsync(LodState.MemberReports, lodCase))
    .Permit(LodTrigger.ConditionReported, LodState.LodInitiation)
    .OnExitAsync(async () => await OnStateExitAsync(LodState.MemberReports, lodCase));

// ✅ DO: Use conditional transitions for branching logic
stateMachine.Configure(LodState.CommanderReview)
    .PermitIf(LodTrigger.ReviewFinished, LodState.OptionalLegal, 
        () => lodCase.RequiresLegalReview)
    .PermitIf(LodTrigger.ReviewFinished, LodState.BoardAdjudication, 
        () => !lodCase.RequiresLegalReview);

// ❌ DON'T: Use string-based states or hardcoded conditions
```

### Service Layer Conventions

```csharp
// ✅ DO: Follow interface-based design
public interface ILodStateMachineService
{
    Task<InformalLineOfDuty> CreateNewCaseAsync(string memberName, string cardId);
    Task<InformalLineOfDuty> TransitionAsync(int caseId, LodTrigger trigger);
    Task<InformalLineOfDuty?> GetCaseAsync(int id);
}

// ✅ DO: Use async/await for all I/O operations
public async Task<InformalLineOfDuty> TransitionAsync(int caseId, LodTrigger trigger)
{
    var lodCase = await _dataService.GetCaseAsync(caseId);
    // Implementation...
}

// ✅ DO: Raise domain events for state changes
await RaiseDomainEventAsync(new StateChangedEvent
{
    CaseId = lodCase.Id,
    FromState = previousState,
    ToState = lodCase.CurrentState,
    Trigger = trigger
});
```

### Domain Models

```csharp
// ✅ DO: Use proper entity modeling with navigation properties
public class InformalLineOfDuty
{
    public int Id { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public LodState CurrentState { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    
    // Navigation properties
    public List<StateTransitionHistory> TransitionHistory { get; set; } = new();
}

// ✅ DO: Track transition history
public class StateTransitionHistory
{
    public int Id { get; set; }
    public int InformalLineOfDutyId { get; set; }
    public LodState FromState { get; set; }
    public LodState ToState { get; set; }
    public LodTrigger Trigger { get; set; }
    public DateTime TransitionDate { get; set; }
}
```

### Blazor Components

```csharp
// ✅ DO: Use Radzen components for UI
<RadzenCard Variant="Variant.Outlined">
    <RadzenStack Gap="1rem">
        <RadzenText TextStyle="TextStyle.H5">LOD Case Details</RadzenText>
        <!-- Content -->
    </RadzenStack>
</RadzenCard>

// ✅ DO: Use Material 3 icon system
<RadzenIcon Icon="account_tree" IconStyle="IconStyle.Primary" />

// ✅ DO: Inject services properly
@inject ILodStateMachineService LodService
@inject IDialogService DialogService
@inject ITooltipService TooltipService
```

### Testing Conventions

```csharp
// ✅ DO: Use xUnit with descriptive test names
[Fact]
public async Task TransitionAsync_ValidTrigger_UpdatesStateAndRecordsHistory()
{
    // Arrange
    var service = CreateService();
    var lodCase = await service.CreateNewCaseAsync("Test Member", "12345");
    
    // Act
    var result = await service.TransitionAsync(lodCase.Id, LodTrigger.ProcessInitiated);
    
    // Assert
    Assert.Equal(LodState.MemberReports, result.CurrentState);
    Assert.Single(result.TransitionHistory);
}

// ✅ DO: Test edge cases and validation
[Fact]
public async Task TransitionAsync_InvalidTrigger_ThrowsInvalidOperationException()
{
    // Test implementation...
}
```

## Naming Conventions

### Files and Folders

- **Models**: `InformalLineOfDuty.cs`, `FormalLineOfDuty.cs`, `Enums.cs`
- **Services**: `I{Feature}Service.cs` and `{Feature}Service.cs`
- **Factories**: `{Feature}StateMachineFactory.cs`
- **Events**: `{Event}Event.cs` (e.g., `StateChangedEvent.cs`)
- **Pages**: `{PageName}.razor` with code-behind `{PageName}.razor.cs`
- **Tests**: `{ClassName}Tests.cs`

### Code Elements

- **Enums**: PascalCase (e.g., `LodState.MemberReports`)
- **Interfaces**: `I{Name}` prefix (e.g., `ILodStateMachineService`)
- **Private fields**: `_camelCase` with underscore prefix
- **Async methods**: `{Name}Async` suffix
- **Domain events**: Inherit from `BaseEvent`

## Common Patterns

### Creating a New Module

When adding a new LOD process module (like FormalLOD):

1. Create folder structure: `Models/`, `Events/`, `Services/`, `Tests/`
2. Define enums for States and Triggers
3. Create domain model inheriting common properties
4. Implement `I{Module}StateMachineFactory` and factory class
5. Implement `I{Module}StateMachineService` and service class
6. Create domain events for state changes
7. Register services in `Program.cs`
8. Write unit tests

### State Machine Configuration Template

```csharp
public StateMachine<TState, TTrigger> CreateStateMachine(TEntity entity)
{
    var stateMachine = new StateMachine<TState, TTrigger>(entity.CurrentState);
    
    // Configure each state
    stateMachine.Configure(TState.Start)
        .Permit(TTrigger.ProcessInitiated, TState.NextState)
        .OnExitAsync(async () => await OnStateExitAsync(TState.Start, entity));
        
    stateMachine.Configure(TState.NextState)
        .OnEntryAsync(async () => await OnStateEntryAsync(TState.NextState, entity))
        .Permit(TTrigger.NextTrigger, TState.FinalState)
        .OnExitAsync(async () => await OnStateExitAsync(TState.NextState, entity));
        
    return stateMachine;
}
```

## Technology-Specific Guidance

### Entity Framework Core

- Use `DbContextFactory<LodDbContext>` for Blazor WebAssembly
- In-memory database is used (SQLite doesn't work in browser)
- Always use async methods (`ToListAsync`, `FirstOrDefaultAsync`, etc.)
- Include navigation properties with `.Include()` when needed

### Radzen Blazor

- Use Material 3 theme (`material3-base.css`)
- Leverage `RadzenStack`, `RadzenCard`, `RadzenDataGrid` for layout
- Use `DialogService` for modals/popups
- Use `TooltipService` for contextual help
- Use `NotificationService` for user feedback

### Stateless Library

- Define enum-based states and triggers
- Use `Configure()` to set up state behaviors
- Use `PermitIf()` for conditional transitions
- Use `OnEntryAsync()` / `OnExitAsync()` for side effects
- Use `Fire()` or `FireAsync()` to trigger transitions

## Performance Considerations

- Use `IDbContextFactory` for proper DbContext lifecycle in Blazor
- Dispose of DbContext instances in `using` blocks
- Use `ValueTask` for frequently called async methods
- Minimize state machine recreation - cache when appropriate
- Use `.AsNoTracking()` for read-only queries

## Security & Validation

- Validate state transitions before firing triggers
- Use `TransitionValidator` classes for business rule validation
- Implement proper error handling with try-catch blocks
- Log state changes for audit trail
- Use domain events for cross-cutting concerns

## Documentation Standards

- Use XML comments for public APIs
- Provide summary descriptions for classes and methods
- Document state machine flows in Mermaid diagrams
- Keep README.md and IMPROVEMENTS.md up to date
- Use clear, descriptive commit messages

## Common Gotchas

1. **DbContext Lifetime**: In Blazor WASM, always create new DbContext from factory
2. **Async All the Way**: Don't mix sync and async code
3. **State Machine Mutability**: State machines are mutable; don't share instances
4. **Enum Changes**: Changing enum values requires database migration
5. **In-Memory Database**: Data is lost on app refresh

## References

- [Stateless Documentation](https://github.com/dotnet-state-machine/stateless)
- [Radzen Blazor Components](https://blazor.radzen.com/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Blazor WebAssembly](https://docs.microsoft.com/en-us/aspnet/core/blazor/)

## Current Development Focus

- ✅ InformalLOD module fully implemented
- 🚧 FormalLOD module in progress
- 📋 Future: Additional LOD process types
- 📋 Future: Real backend API integration
- 📋 Future: Authentication and authorization
