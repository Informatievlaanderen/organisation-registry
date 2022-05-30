namespace OrganisationRegistry.Infrastructure.Events;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Authorization;

public interface IEventStore
{
    Task Save<T>(IEnumerable<IEvent> events, IUser user);

    IEnumerable<IEvent> Get<T>(Guid aggregateId, int fromVersion);

    IEnumerable<IEnvelope> GetEventEnvelopes<T>(Guid aggregateId);
    IEnumerable<IEnvelope> GetEventEnvelopes(params Type[] eventTypes);
    IEnumerable<IEnvelope> GetEventEnvelopesUntil<T>(Guid aggregateId, int untilEventNumber);
    IEnumerable<IEnvelope> GetEventEnvelopesAfter(int eventNumber);
    IEnumerable<IEnvelope> GetEventEnvelopesAfter(int eventNumber, int maxEvents, params Type[] eventsBeingListenedTo);

    int GetEventEnvelopeCount(DateTimeOffset? dateTimeOffset = null);

    int GetLastEvent();
}