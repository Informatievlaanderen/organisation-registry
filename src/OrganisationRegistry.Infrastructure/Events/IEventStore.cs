namespace OrganisationRegistry.Infrastructure.Events
{
    using System;
    using System.Collections.Generic;

    public interface IEventStore
    {
        void Save<T>(IEnumerable<IEvent> events);

        IEnumerable<IEvent> Get<T>(Guid aggregateId, int fromVersion);

        IEnumerable<IEnvelope> GetEventEnvelopes(params Type[] eventTypes);

        IEnumerable<IEnvelope> GetEventEnvelopesAfter(int eventNumber);
        IEnumerable<IEnvelope> GetEventEnvelopesAfter(int eventNumber, int maxEvents, params Type[] eventsBeingListenedTo);

        int GetEventEnvelopeCount(DateTimeOffset? dateTimeOffset = null);

        int GetLastEvent();
    }
}
