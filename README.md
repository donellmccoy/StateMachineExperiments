# StateMachineExperiments

A demonstration of implementing a Line of Duty (LOD) state machine using Stateless library and Entity Framework Core with SQLite for persistence.

## Overview

This project demonstrates a complete state machine implementation for managing Air Force Reserve Command (AFRC) Informal Line of Duty determination cases using:

- **Stateless**: A hierarchical state machine library for .NET
- **Entity Framework Core**: For data persistence and tracking
- **SQLite**: Lightweight database for state and transition history

## Features

- Complete LOD workflow with configurable state transitions

- Persistent state storage using Entity Framework Core

- Full transition history tracking
- Authority mapping for each state
- Conditional branching (optional Legal/Wing reviews)
- Resume capability - cases can be paused and resumed
- Multiple scenario demonstrations

## State Machine Workflow

```mermaid
%%{init: {'theme':'base', 'themeVariables': { 'primaryColor':'#FFB84D','primaryTextColor':'#000','primaryBorderColor':'#E69500','lineColor':'#333','secondaryColor':'#5DCCCC','tertiaryColor':'#FF6B9D','fontSize':'16px'}}}%%
stateDiagram-v2
    direction TB
    
    [*] --> Start
    
    Start --> MemberReports:    Valid Card    
    Start --> End:    Invalid Card    
    
    MemberReports --> LodInitiation:    Report Submitted    
    
    LodInitiation --> MedicalAssessment:    Case Opened    
    LodInitiation --> End:    Incomplete Info    
    
    MedicalAssessment --> CommanderReview:    Assessment Complete    
    
    state decision1 <<choice>>
    CommanderReview --> decision1:    Review Complete    
    decision1 --> OptionalLegal:    Legal Review Needed    
    decision1 --> BoardAdjudication:    Fast Track    
    
    state decision2 <<choice>>
    OptionalLegal --> decision2:    Legal Review Done    
    decision2 --> OptionalWing:    Wing Review Needed    
    decision2 --> BoardAdjudication:    Proceed to Board    
    
    OptionalWing --> BoardAdjudication:    Wing Approved    
    
    BoardAdjudication --> Determination:    Board Decision    
    
    Determination --> Notification:    Determination Made    
    
    state decision3 <<choice>>
    Notification --> decision3:    Member Notified    
    decision3 --> Appeal:    Appeal Filed    
    decision3 --> End:    No Appeal    
    
    Appeal --> End:    Appeal Resolved    
    
    End --> [*]
    
    state "    Start    " as Start
    state "    Member Reports    " as MemberReports
    state "    LOD Initiation    " as LodInitiation
    state "    Medical Assessment    " as MedicalAssessment
    state "    Commander Review    " as CommanderReview
    state "    Legal Review    " as OptionalLegal
    state "    Wing Review    " as OptionalWing
    state "    Board Adjudication    " as BoardAdjudication
    state "    Determination    " as Determination
    state "    Notification    " as Notification
    state "    Appeal    " as Appeal
    state "    End    " as End
    
    classDef entry fill:#FFB84D,stroke:#E69500,stroke-width:3px,color:#000
    classDef process fill:#5DCCCC,stroke:#3AAFAF,stroke-width:2px,color:#000
    classDef optional fill:#FF6B9D,stroke:#E5558A,stroke-width:2px,color:#000
    classDef decision fill:#B19CD9,stroke:#9A7FC7,stroke-width:2px,color:#000
    classDef complete fill:#FF6B6B,stroke:#E55555,stroke-width:3px,color:#fff
    
    class Start,MemberReports entry
    class LodInitiation,MedicalAssessment,BoardAdjudication,Determination,Notification process
    class CommanderReview,OptionalLegal,OptionalWing optional
    class Appeal decision
    class End complete
```

### Legend

- 🟠 **Orange**: Entry states (card read, member reporting)
- 🔵 **Cyan**: Required processing states
- 💗 **Pink**: Optional review states (conditional paths)
- 🟣 **Purple**: Appeal/decision handling
- 🔴 **Red**: Terminal state (case closed)
- ◆ **Diamond**: Decision points (branching logic)

### Workflow Phases

#### 1. Entry & Reporting

- `Start` → `MemberReports` → `LodInitiation`

#### 2. Assessment Phase

- `MedicalAssessment` → `CommanderReview`

#### 3. Review & Optional Paths

- `OptionalLegal` (conditional)
- `OptionalWing` (conditional)
- `BoardAdjudication` (required)

#### 4. Determination & Finalization

- `Determination` → `Notification`

#### 5. Resolution

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
