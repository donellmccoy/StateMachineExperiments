using StateMachineExperiments.Common.Infrastructure;
using System;

namespace StateMachineExperiments.Modules.FormalLOD.Events
{
    public class FormalLodStateChangedEvent : DomainEvent
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string FromState { get; set; } = string.Empty;
        public string ToState { get; set; } = string.Empty;
        public string Trigger { get; set; } = string.Empty;
        public string Authority { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class FormalLodCaseCreatedEvent : DomainEvent
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string? MemberId { get; set; }
        public string? MemberName { get; set; }
        public bool IsDeathCase { get; set; }
    }

    public class FormalLodAppealFiledEvent : DomainEvent
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public DateTime AppealDate { get; set; }
        public string? MemberId { get; set; }
        public bool IsDeathCase { get; set; } // Death cases have 180-day appeal window
    }

    public class FormalLodDeterminationFinalizedEvent : DomainEvent
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string Determination { get; set; } = string.Empty;
        public string ApprovingAuthority { get; set; } = string.Empty;
    }

    public class FormalLodInvestigationStartedEvent : DomainEvent
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string InvestigatingOfficerId { get; set; } = string.Empty;
        public string InvestigatingOfficerName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public bool IsDeathCase { get; set; }
    }

    public class FormalLodToxicologyReceivedEvent : DomainEvent
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public DateTime ReceivedDate { get; set; }
    }
}
