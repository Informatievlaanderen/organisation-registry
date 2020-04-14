namespace OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Events;

    internal class SpecEventStorage : IEventStore
    {
        private readonly IEventPublisher _publisher;

        public List<IEvent> Events { get; set; }

        public SpecEventStorage(IEventPublisher publisher, List<IEvent> events)
        {
            _publisher = publisher;
            Events = events;
        }

        public async Task Save<T>(IEnumerable<IEvent> events)
        {
            var eventList = events as IList<IEvent> ?? events.ToList();
            Events.AddRange(eventList);
            foreach (var @event in eventList)
                _publisher.Publish(null, null, (dynamic)@event.ToEnvelope());
        }

        public IEnumerable<IEvent> Get<T>(Guid aggregateId, int fromVersion)
            => Events.Where(x => x.Version > fromVersion && x.Id == aggregateId);

        public IEnumerable<IEnvelope> GetEventEnvelopes(params Type[] eventTypes)
            => Events.Where(x => eventTypes.Contains(x.GetType())).Select(x => x.ToEnvelope());

        public IEnumerable<IEnvelope> GetEventEnvelopesAfter(int eventNumber)
            => throw new NotImplementedException();

        public IEnumerable<IEnvelope> GetEventEnvelopesAfter(int eventNumber, int maxEvents, params Type[] eventsBeingListenedTo)
            => throw new NotImplementedException();

        public int GetEventEnvelopeCount(DateTimeOffset? dateTimeOffset = null)
            => throw new NotImplementedException();

        public int GetLastEvent()
            => throw new NotImplementedException();
    }
}
