# Module Structure

## Overview

This project is organized into a modular architecture that separates concerns between different Line of Duty (LOD) processes and shared infrastructure.

## Directory Structure

```
StateMachineExperiments/
├── Common/                           # Shared infrastructure and cross-cutting concerns
│   ├── Data/                        # Database context and factory
│   │   ├── LodDbContext.cs
│   │   └── LodDbContextFactory.cs
│   ├── Exceptions/                  # Custom exception types
│   │   └── LodExceptions.cs
│   └── Infrastructure/              # Base infrastructure components
│       ├── DomainEventBase.cs      # Base class for domain events
│       ├── InMemoryEventPublisher.cs  # Event publishing implementation
│       └── ValidationResult.cs      # Standard validation result pattern
│
├── Modules/                         # Business modules
│   ├── InformalLOD/                # Informal Line of Duty process
│   │   ├── Events/                 # Domain events specific to informal LOD
│   │   │   └── DomainEvents.cs
│   │   ├── Models/                 # Domain models
│   │   │   ├── Enums.cs           # LodState, LodTrigger, LodAuthority
│   │   │   ├── InformalLineOfDuty.cs
│   │   │   └── StateTransitionHistory.cs
│   │   ├── Services/               # Business logic and state machine
│   │   │   ├── ILodDataService.cs
│   │   │   ├── LodDataService.cs
│   │   │   ├── ILodBusinessRuleService.cs
│   │   │   ├── LodBusinessRuleService.cs
│   │   │   ├── ILodStateMachineService.cs
│   │   │   ├── LodStateMachineService.cs
│   │   │   ├── LodStateMachineFactory.cs
│   │   │   ├── LodTransitionValidator.cs
│   │   │   └── LodVisualizationService.cs
│   │   └── Tests/                  # Unit tests
│   │       ├── LodStateMachineServiceTests.cs
│   │       └── LodBusinessRuleServiceTests.cs
│   │
│   └── FormalLOD/                  # Formal Line of Duty process (to be implemented)
│       ├── Events/
│       ├── Models/
│       ├── Services/
│       └── Tests/
│
├── Migrations/                      # Entity Framework migrations
├── Program.cs                       # Application entry point
└── StateMachineExperiments.csproj  # Project file
```

## Namespace Organization

### Common Namespaces
- `StateMachineExperiments.Common.Data` - Database access layer
- `StateMachineExperiments.Common.Exceptions` - Shared exception types
- `StateMachineExperiments.Common.Infrastructure` - Base infrastructure components

### Module Namespaces
- `StateMachineExperiments.Modules.InformalLOD.Models` - Informal LOD domain models
- `StateMachineExperiments.Modules.InformalLOD.Events` - Informal LOD domain events
- `StateMachineExperiments.Modules.InformalLOD.Services` - Informal LOD business logic
- `StateMachineExperiments.Modules.InformalLOD.Tests` - Informal LOD unit tests

## Common Components

### Exceptions
Located in `Common/Exceptions/LodExceptions.cs`:
- `CaseNotFoundException` - Thrown when a case ID is not found
- `InvalidStateTransitionException` - Thrown when a trigger is not permitted in current state
- `MissingRequiredDataException` - Thrown when required fields are missing
- `TransitionValidationException` - Thrown when validation rules fail

### Infrastructure
Located in `Common/Infrastructure/`:
- `DomainEvent` - Abstract base class for all domain events
- `IEventPublisher` - Interface for publishing domain events
- `InMemoryEventPublisher` - In-memory implementation of event publisher
- `ValidationResult` - Standard result pattern for validation

### Data Access
Located in `Common/Data/`:
- `LodDbContext` - Entity Framework database context
- `LodDbContextFactory` - Factory for creating database contexts

## InformalLOD Module

### Purpose
Implements the informal Line of Duty determination process, which handles cases that don't require a formal board proceeding.

### State Machine
The informal LOD process supports 12 states:
1. `Start` - Initial state
2. `Processing` - Case initiated
3. `Initiation` - Condition reported
4. `Assessment` - Medical assessment in progress
5. `CommanderReview` - Commander reviewing
6. `LegalReview` - Legal review (optional, based on business rules)
7. `WingReview` - Wing commander review (optional, based on business rules)
8. `Adjudication` - Making determination
9. `Determination` - Determination made
10. `Notification` - Member notified
11. `Appeal` - Appeal filed
12. `End` - Process complete

### Business Rules
Located in `LodBusinessRuleService`:
- Automatic determination of required reviews based on injury severity and cost
- Appeal eligibility validation (30-day window after notification)
- Rule application based on case properties

### Domain Events
Located in `Modules/InformalLOD/Events/DomainEvents.cs`:
- `LodCaseCreatedEvent` - Published when a new case is created
- `LodStateChangedEvent` - Published on every state transition
- `LodDeterminationFinalizedEvent` - Published when determination is finalized
- `LodAppealFiledEvent` - Published when an appeal is filed

### Key Services
- `LodStateMachineService` - Main orchestrator for state transitions
- `LodDataService` - Data access abstraction
- `LodBusinessRuleService` - Business rule engine
- `LodTransitionValidator` - Pre-flight validation
- `LodStateMachineFactory` - Creates configured state machines with entry/exit actions
- `LodVisualizationService` - Generates DOT graphs for visualization

## FormalLOD Module

### Status
**Not yet implemented** - Placeholder structure created for future formal LOD process

### Planned Structure
The formal LOD module will follow the same organizational pattern as InformalLOD but will implement the formal board process with:
- Different state transitions (board proceedings, evidence collection, etc.)
- Different business rules (board composition requirements, etc.)
- Different validation logic
- Potentially shared infrastructure from Common/

## Benefits of Modular Architecture

1. **Separation of Concerns** - Informal and formal processes are isolated
2. **Shared Infrastructure** - Common components eliminate duplication
3. **Testability** - Each module has its own test suite
4. **Maintainability** - Changes to one module don't affect the other
5. **Scalability** - Easy to add new LOD process types as additional modules
6. **Clear Dependencies** - Modules depend on Common, but not on each other

## Migration Notes

The project was restructured from a flat structure to this modular architecture:
- All service files moved to `Modules/InformalLOD/Services/`
- All model files moved to `Modules/InformalLOD/Models/`
- Exceptions and infrastructure extracted to `Common/`
- Namespaces updated throughout the codebase
- Tests moved to `Modules/InformalLOD/Tests/`
- All 18 unit tests continue to pass after restructuring
