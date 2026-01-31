# StateMachineExperiments

A demonstration of implementing a Line of Duty (LOD) state machine using Stateless library and Entity Framework Core with SQLite for persistence.

## Overview

This project demonstrates a complete state machine implementation for managing Air Force Reserve Command (AFRC) Informal Line of Duty determination cases using:

- **Stateless**: A hierarchical state machine library for .NET
- **Entity Framework Core**: For data persistence and tracking
- **SQLite**: Lightweight database for state and transition history

## Features

-  Complete LOD workflow with configurable state transitions
-  Persistent state storage using Entity Framework Core
-  Full transition history tracking
-  Authority mapping for each state
-  Conditional branching (optional Legal/Wing reviews)
-  Resume capability - cases can be paused and resumed
-  Multiple scenario demonstrations

## State Machine Workflow

```mermaid
%%{init: {'theme':'base', 'themeVariables': { 'primaryColor':'#0066cc','primaryTextColor':'#fff','primaryBorderColor':'#004999','lineColor':'#666','secondaryColor':'#28a745','tertiaryColor':'#ffc107'}}}%%
stateDiagram-v2
    direction TB
    
    [*] --> Start
    
    Start --> MemberReports:   Process Initiated   
    MemberReports --> LodInitiation:   Condition Reported   
    LodInitiation --> MedicalAssessment:   Initiation Complete   
    MedicalAssessment --> CommanderReview:   Assessment Done   
    
    CommanderReview --> OptionalLegal:   Review Finished   
    CommanderReview --> BoardAdjudication:   Review Finished (fast-track)   
    
    OptionalLegal --> OptionalWing:   Legal Done   
    OptionalLegal --> BoardAdjudication:   Legal Done   
    
    OptionalWing --> BoardAdjudication:   Wing Done   
    
    BoardAdjudication --> Determination:   Adjudication Complete   
    Determination --> Notification:   Determination Finalized   
    
    Notification --> Appeal:   Appeal Requested   
    Notification --> End:   No Appeal Requested   
    
    Appeal --> End:   Appeal Resolved   
    End --> [*]
    
    state "   Start   " as Start
    state "   Member Reports   " as MemberReports
    state "   LOD Initiation   " as LodInitiation
    state "   Medical Assessment   " as MedicalAssessment
    state "   Commander Review   " as CommanderReview
    state "   Legal Review   " as OptionalLegal
    state "   Wing Review   " as OptionalWing
    state "   Board Adjudication   " as BoardAdjudication
    state "   Determination   " as Determination
    state "   Notification   " as Notification
    state "   Appeal   " as Appeal
    state "   End   " as End
    
    classDef optional fill:#ffc107,stroke:#ff9800,stroke-width:3px,color:#000
    classDef required fill:#0066cc,stroke:#004999,stroke-width:2px,color:#fff
    classDef decision fill:#28a745,stroke:#1e7e34,stroke-width:2px,color:#fff
    classDef terminal fill:#6c757d,stroke:#495057,stroke-width:2px,color:#fff
    
    class OptionalLegal,OptionalWing optional
    class MemberReports,LodInitiation,MedicalAssessment,BoardAdjudication,Determination,Notification required
    class CommanderReview,Appeal decision
    class Start,End terminal
```

### Legend

- 🔵 **Blue**: Required workflow states
- 🟢 **Green**: Decision points with branching logic
- 🟡 **Yellow**: Optional review states (conditional)
- ⚫ **Gray**: Terminal states (start/end)

### Workflow Phases

**1. Entry & Reporting**
- `Start` → `MemberReports` → `LodInitiation`

**2. Assessment Phase**
- `MedicalAssessment` → `CommanderReview`

**3. Review & Optional Paths**
- `OptionalLegal` (conditional)
- `OptionalWing` (conditional)
- `BoardAdjudication` (required)

**4. Determination & Finalization**
- `Determination` → `Notification`

**5. Resolution**
- `Appeal` (optional) → `End`

### State Machine Components

**States** (workflow positions):
- **Entry**: `Start`, `MemberReports`, `LodInitiation`
- **Assessment**: `MedicalAssessment`, `CommanderReview`
- **Optional Reviews**: `OptionalLegal`, `OptionalWing`
- **Determination**: `BoardAdjudication`, `Determination`, `Notification`
- **Resolution**: `Appeal`, `End`

**Triggers** (transition events):
- `ProcessInitiated`, `ConditionReported`, `InitiationComplete`
- `AssessmentDone`, `ReviewFinished`, `LegalDone`, `WingDone`
- `AdjudicationComplete`, `DeterminationFinalized`
- `AppealRequested`, `NoAppealRequested`, `AppealResolved`

## Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- Visual Studio 2022 or VS Code

### Running the Application

```powershell
cd StateMachineExperiments
dotnet restore
dotnet run
```

### Database

The application uses SQLite and creates lod_cases.db automatically. The database schema is managed through EF Core migrations.

## Project Structure

- **Models/** - Domain entities (InformalLineOfDuty, StateTransitionHistory, Enums)
- **Services/** - Business logic (LodStateMachineService)
- **Data/** - EF Core DbContext and migrations
- **Program.cs** - Demo scenarios

## Scenarios Demonstrated

1. **Full workflow** with all optional reviews and appeal
2. **Fast-track** case without optional reviews
3. **Persistence & resumability** demonstration

## Technologies

- C# 12
- .NET 10.0
- Stateless 5.20.0
- Entity Framework Core 10.0.2
- SQLite

## License

This is a demonstration project for educational purposes.
