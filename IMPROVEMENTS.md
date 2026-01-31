# State Machine Experiments - LOD Workflow

A comprehensive Line of Duty (LOD) case management system built with the Stateless state machine library and Entity Framework Core, demonstrating advanced state machine patterns and best practices.

## Features Implemented

### ✅ 1. Custom Exception Types
Located in `Common/Exceptions/LodExceptions.cs`:
- **CaseNotFoundException** - Thrown when a case is not found
- **InvalidStateTransitionException** - Thrown when attempting invalid state transitions
- **MissingRequiredDataException** - Thrown when required data is missing
- **TransitionValidationException** - Thrown when validation fails

### ✅ 2. Optimistic Concurrency Control
- Added `RowVersion` property to `InformalLineOfDuty` model
- Prevents race conditions when multiple users work on the same case
- Automatic detection of concurrent updates via EF Core

### ✅ 3. Business Rule Service
`Modules/InformalLOD/Services/LodBusinessRuleService.cs` implements:
- **Automatic Legal Review Detection**: Required if injury severity > 5 OR cost > $50,000
- **Automatic Wing Review Detection**: Required if injury severity > 7 OR cost > $100,000
- **Appeal Eligibility**: Must be filed within 30 days of notification
- **Encapsulated Business Logic**: Centralized decision-making

### ✅ 4. Domain Events
`Modules/InformalLOD/Events/DomainEvents.cs` provides event-driven architecture:
- **LodCaseCreatedEvent** - Published when a new case is created
- **LodStateChangedEvent** - Published on every state transition
- **LodDeterminationFinalizedEvent** - Published when determination is made
- **LodAppealFiledEvent** - Published when appeal is filed
- **IEventPublisher** interface (in `Common/Infrastructure/`) for integration with external systems

### ✅ 5. Validation Layer
`Modules/InformalLOD/Services/LodTransitionValidator.cs`:
- Pre-transition validation
- Business rule enforcement
- Data completeness checks
- Appeal deadline validation

### ✅ 6. State Machine Factory
`Modules/InformalLOD/Services/LodStateMachineFactory.cs`:
- Centralized state machine configuration
- Lifecycle management
- Support for reusable state machine instances
- Configured with entry/exit actions

### ✅ 7. State Entry/Exit Actions
Automatic actions on state changes:
- **OnEntry** - Automatic notifications, logging
- **OnExit** - State cleanup, event publishing
- Example: Automatic email notification when entering Notification state

### ✅ 8. Trigger Parameters Support
Infrastructure in place for parameterized triggers:
- `ReviewData` class for passing review information
- Extensible for future trigger parameters

### ✅ 9. State Machine Visualization
`Modules/InformalLOD/Services/LodVisualizationService.cs`:
- Generates DOT format diagrams
- Saves to file for Graphviz rendering
- Command: `dot -Tpng lod_state_machine.dot -o diagram.png`

### ✅ 10. Comprehensive Unit Tests
18 passing tests in `Modules/InformalLOD/Tests/`:
- **LodStateMachineServiceTests.cs** - 6 tests
  - Case creation and event publishing
  - Valid/invalid transitions
  - Conditional routing (legal/wing reviews)
  - Permitted triggers
  
- **LodBusinessRuleServiceTests.cs** - 12 tests
  - Legal review requirements (6 scenarios)
  - Wing review requirements (6 scenarios)
  - Appeal eligibility validation
  - Automatic business rule application

### ✅ 11. Modular Architecture
Project restructured into modules for separation of concerns:
- **Common/** - Shared infrastructure, exceptions, and data access
- **Modules/InformalLOD/** - Informal LOD process implementation
- **Modules/FormalLOD/** - Placeholder for formal LOD process
- See `MODULE_STRUCTURE.md` for detailed documentation

## Architecture

### Modular Organization
```
Common/
  ├── Data/                    # Shared database context
  ├── Exceptions/              # Shared exception types
  └── Infrastructure/          # Base classes (events, validation)

Modules/
  ├── InformalLOD/            # Informal LOD process
  │   ├── Models/              # Domain models and enums
  │   ├── Events/              # Domain events
  │   ├── Services/            # Business logic and state machine
  │   └── Tests/               # Unit tests
  └── FormalLOD/              # Future: Formal LOD process
      └── README.md            # Implementation guidance
```

### Dependency Injection
```csharp
ILodDataService              -> LodDataService
ILodBusinessRuleService      -> LodBusinessRuleService
ILodTransitionValidator      -> LodTransitionValidator
ILodStateMachineFactory      -> LodStateMachineFactory
IEventPublisher              -> InMemoryEventPublisher
ILodVisualizationService     -> LodVisualizationService
ILodStateMachineService      -> LodStateMachineService
```

### State Flow
```
Start → MemberReports → LodInitiation → MedicalAssessment → CommanderReview
  ↓ (if legal review required)
  OptionalLegal
    ↓ (if wing review required)
    OptionalWing
      ↓
  BoardAdjudication → Determination → Notification
    ↓ (if appeal filed)
    Appeal → End
    ↓ (if no appeal)
    End
```

### Business Rules
- **Severity > 5 OR Cost > $50k** → Legal Review Required
- **Severity > 7 OR Cost > $100k** → Wing Review Required  
- **Appeal within 30 days** → Appeal Eligible

## Running the Project

### Build & Test
```bash
dotnet build
dotnet test  # All 18 tests should pass
```

### Run Demonstrations
```bash
dotnet run
```

The program demonstrates:
1. **Scenario 1:** Full workflow with all optional reviews and appeal
2. **Scenario 2:** Fast-track case skipping optional reviews
3. **Scenario 3:** Persistence and resumability
4. **Validation demonstration:** Pre-flight validation checks
5. **Visualization:** DOT graph generation
6. **Events:** Summary of published domain events

## Database

SQLite database: `lod_cases.db`

Migrations:
- `InitialCreate` - Base schema
- `AddBusinessDecisionFlags` - Business rule properties
- `AddRowVersionForConcurrency` - Optimistic concurrency

## Test Results

```
✅ 18/18 tests passing
- CreateNewCaseAsync_ShouldCreateCaseAndPublishEvent
- FireTriggerAsync_WithValidTransition_ShouldUpdateState
- FireTriggerAsync_WithInvalidTransition_ShouldThrowException
- CommanderReview_WithLegalReviewRequired_ShouldRouteToLegal
- GetPermittedTriggersAsync_ShouldReturnCorrectTriggers
- RequiresLegalReview (6 theory tests)
- RequiresWingReview (6 theory tests)
- IsAppealEligible_WithinDeadline_ShouldReturnTrue
- IsAppealEligible_AfterDeadline_ShouldReturnFalse
- ApplyBusinessRules_ShouldSetCorrectFlags
```

## Key Improvements from Original

1. **Separation of Concerns** - Data access, business rules, validation all separated
2. **Testability** - Full unit test coverage with mocking
3. **Dynamic Guards** - Conditions evaluated from case data, not hardcoded flags
4. **Event-Driven** - Integration points via domain events
5. **Validation** - Pre-flight checks prevent invalid transitions
6. **Type Safety** - Custom exceptions for better error handling
7. **Visualization** - Generate diagrams for documentation
8. **Concurrency** - Optimistic locking prevents conflicts
9. **Maintainability** - Business rules in one place, easy to modify

## Technologies

- .NET 10.0
- Entity Framework Core 10.0
- Stateless 5.20.0
- xUnit 2.9.2
- Moq 4.20.72
- SQLite

## License

MIT
