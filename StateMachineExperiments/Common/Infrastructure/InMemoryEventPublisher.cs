using System;
using System.Collections.Generic;

namespace StateMachineExperiments.Common.Infrastructure
{
    public class InMemoryEventPublisher : IEventPublisher
    {
        private readonly List<DomainEvent> _events = new();

        public void Publish<TEvent>(TEvent domainEvent) where TEvent : DomainEvent
        {
            _events.Add(domainEvent);
            Console.WriteLine($"[EVENT] {domainEvent.GetType().Name} published at {domainEvent.OccurredAt:HH:mm:ss}");
        }

        public void PublishAll(IEnumerable<DomainEvent> domainEvents)
        {
            foreach (var evt in domainEvents)
            {
                Publish(evt);
            }
        }

        public IReadOnlyList<DomainEvent> GetPublishedEvents() => _events.AsReadOnly();
        
        public void ClearEvents() => _events.Clear();
    }
}
