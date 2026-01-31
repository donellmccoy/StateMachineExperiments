# FormalLOD Module (Future Implementation)

## Purpose
This module will implement the **Formal Line of Duty** determination process, which handles cases requiring a formal investigative board proceeding.

## Differences from InformalLOD

### Process Characteristics
- **InformalLOD**: Commander-level determination, faster process, simpler cases
- **FormalLOD**: Board-level determination, comprehensive investigation, complex cases

### Key Distinctions

#### States
Formal LOD will likely include states such as:
- Board Convening
- Evidence Collection
- Witness Testimony
- Board Deliberation
- Board Findings
- Reviewing Authority Actions
- Final Authority Approval
- Appeal to Higher Authority

#### Business Rules
- Board composition requirements (minimum members, rank requirements)
- Evidence chain of custody
- Testimony scheduling
- Timeline requirements for proceedings
- Authority levels for approval

#### Triggers
Different triggers will apply:
- ConveneBoard
- CollectEvidence
- ScheduleWitness
- ConductHearing
- DeliberateFindings
- SubmitFindings
- ReviewerAction
- FinalApproval

## Proposed Structure

```
FormalLOD/
├── Events/
│   └── FormalLodDomainEvents.cs      # Board convened, evidence collected, etc.
├── Models/
│   ├── FormalLodEnums.cs             # FormalLodState, FormalLodTrigger
│   ├── FormalLineOfDuty.cs           # Main case entity
│   ├── BoardMember.cs                # Board composition tracking
│   ├── Evidence.cs                   # Evidence collection
│   └── Testimony.cs                  # Witness testimony records
├── Services/
│   ├── IFormalLodDataService.cs
│   ├── FormalLodDataService.cs
│   ├── IFormalLodBusinessRuleService.cs
│   ├── FormalLodBusinessRuleService.cs
│   ├── IFormalLodStateMachineService.cs
│   ├── FormalLodStateMachineService.cs
│   ├── FormalLodStateMachineFactory.cs
│   ├── FormalLodTransitionValidator.cs
│   └── FormalLodVisualizationService.cs
└── Tests/
    ├── FormalLodStateMachineServiceTests.cs
    └── FormalLodBusinessRuleServiceTests.cs
```

## Shared Components

The FormalLOD module will leverage common infrastructure:
- `Common.Infrastructure.DomainEvent` - Base class for formal LOD events
- `Common.Infrastructure.IEventPublisher` - Event publishing
- `Common.Infrastructure.ValidationResult` - Validation patterns
- `Common.Exceptions.*` - Shared exception types
- `Common.Data.LodDbContext` - Extended for formal LOD entities

## Implementation Notes

When implementing this module:

1. **Follow the InformalLOD pattern**: Use the same architectural approach for consistency
2. **Reuse common infrastructure**: Don't duplicate base classes or patterns
3. **Add to DbContext**: Update `LodDbContext` to include formal LOD entities
4. **Create migrations**: Add EF migrations for new entities
5. **Implement validators**: Create formal-specific validation rules
6. **Write tests**: Maintain same test coverage as InformalLOD
7. **Update Program.cs**: Add demo scenarios for formal LOD process

## Business Rules to Implement

- Board must have minimum 3 members
- At least one member must be O-3 or above
- President must be at least one rank above the member
- Evidence must be collected within timeline
- All witnesses must be interviewed
- Board findings require majority vote
- Reviewing authority can approve, modify, or disapprove
- Final authority has ultimate approval

## Database Considerations

### New Tables
- `FormalLodCases`
- `FormalLodBoardMembers`
- `FormalLodEvidence`
- `FormalLodTestimony`
- `FormalLodBoardFindings`

### Relationships
- FormalLineOfDuty 1:N BoardMembers
- FormalLineOfDuty 1:N Evidence
- FormalLineOfDuty 1:N Testimony
- FormalLineOfDuty 1:N TransitionHistory (shared)

## Future Enhancements

Consider these features:
- Document attachment support
- Automated timeline tracking
- Email notifications for hearings
- Board member availability checking
- Conflict of interest detection
- Automated finding templates
