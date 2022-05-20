namespace OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Events;

    internal class SpecEventStorage : IEventStore
    {
        private readonly IEventPublisher _publisher;

        public List<IEvent> Events { get; set; }

        public SpecEventStorage(IEventPublisher publisher)
        {
            _publisher = publisher;
            Events = new List<IEvent>();
        }

        public SpecEventStorage(IEventPublisher eventpublisher, List<IEvent> toList)
        {
            _publisher = eventpublisher;
            Events = toList;

        }

        public Task Save<T>(IEnumerable<IEvent> events, IUser user)
        {
            var eventList = events as IList<IEvent> ?? events.ToList();
            Events.AddRange(eventList);
            foreach (var @event in eventList)
                _publisher.Publish(null, null, (dynamic)@event.ToEnvelope());
            return Task.CompletedTask;
        }

        public IEnumerable<IEvent> Get<T>(Guid aggregateId, int fromVersion)
            => Events.Where(x => x.Version > fromVersion && x.Id == aggregateId);

        public IEnumerable<IEnvelope> GetEventEnvelopes<T>(Guid aggregateId)
            => Events.Where(x => x.Id == aggregateId).Select(x => x.ToEnvelope());

        public IEnumerable<IEnvelope> GetEventEnvelopes(params Type[] eventTypes)
            => Events.Where(x => eventTypes.Contains(x.GetType())).Select(x => x.ToEnvelope());

        public IEnumerable<IEnvelope> GetEventEnvelopesUntil<T>(Guid aggregateId, int untilEventNumber)
            => Events.Where(x => x.Version <= untilEventNumber && x.Id == aggregateId).Select(x => x.ToEnvelope());

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
