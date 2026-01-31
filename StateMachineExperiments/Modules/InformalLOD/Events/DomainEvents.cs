using StateMachineExperiments.Common.Infrastructure;
using System;

namespace StateMachineExperiments.Modules.InformalLOD.Events
{
    public class LodStateChangedEvent : DomainEvent
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string FromState { get; set; } = string.Empty;
        public string ToState { get; set; } = string.Empty;
        public string Trigger { get; set; } = string.Empty;
        public string Authority { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class LodCaseCreatedEvent : DomainEvent
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string? MemberId { get; set; }
        public string? MemberName { get; set; }
    }

    public class LodAppealFiledEvent : DomainEvent
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public DateTime AppealDate { get; set; }
        public string? MemberId { get; set; }
    }

    public class LodDeterminationFinalizedEvent : DomainEvent
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string Determination { get; set; } = string.Empty;
        public string ApprovingAuthority { get; set; } = string.Empty;
    }
}
