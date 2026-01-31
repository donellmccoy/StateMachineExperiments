using System;

namespace StateMachineExperiments.Common.Infrastructure
{
    public abstract class DomainEvent
    {
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }

    public interface IEventPublisher
    {
        void Publish<TEvent>(TEvent domainEvent) where TEvent : DomainEvent;
        void PublishAll(System.Collections.Generic.IEnumerable<DomainEvent> domainEvents);
    }
}
