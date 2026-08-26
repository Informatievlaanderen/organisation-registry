namespace OrganisationRegistry.ElasticSearch.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Events;

/// <summary>
/// Append-only in-memory event store, so a test can grow the stream between projection runs.
/// Mirrors <see cref="OrganisationRegistry.Infrastructure.EventStore.SqlServerEventStore"/> where it matters:
/// envelope numbers are 1-based and sequential, and reads filter on aggregate id and event number only.
/// </summary>
public class InMemoryEventStore : IEventStore
{
    private readonly List<IEnvelope> _envelopes = new();

    public IEnvelope Append(IEvent @event)
    {
        var envelope = @event.ToEnvelope(
            _envelopes.Count + 1, string.Empty, string.Empty, string.Empty, string.Empty);

        _envelopes.Add(envelope);

        return envelope;
    }

    public int GetLastEvent()
        => _envelopes.Count;

    public IEnumerable<IEnvelope> GetEventEnvelopesAfter(int eventNumber, int maxEvents, params Type[] eventsBeingListenedTo)
        => _envelopes
            .Where(x => x.Number > eventNumber)
            .Where(x => eventsBeingListenedTo.Length == 0 || eventsBeingListenedTo.Contains(x.Body.GetType()))
            .Take(maxEvents)
            .ToList();

    public IEnumerable<IEnvelope> GetEventEnvelopesUntil<T>(Guid aggregateId, int untilEventNumber)
        => _envelopes
            .Where(x => x.Id == aggregateId && x.Number <= untilEventNumber)
            .ToList();

    public IEnumerable<IEnvelope> GetEventEnvelopesAfter(int eventNumber)
        => _envelopes.Where(x => x.Number > eventNumber).ToList();

    public IEnumerable<IEnvelope> GetEventEnvelopes<T>(Guid aggregateId)
        => _envelopes.Where(x => x.Id == aggregateId).ToList();

    public IEnumerable<IEnvelope> GetEventEnvelopes(params Type[] eventTypes)
        => _envelopes.Where(x => eventTypes.Contains(x.Body.GetType())).ToList();

    public IEnumerable<IEvent> Get<T>(Guid aggregateId, int fromVersion)
        => _envelopes.Where(x => x.Id == aggregateId).Select(x => x.Body).ToList();

    public int GetEventEnvelopeCount(DateTimeOffset? dateTimeOffset = null)
        => _envelopes.Count;

    public Task Save<T>(IEnumerable<IEvent> events, IUser user)
    {
        foreach (var @event in events)
            Append(@event);

        return Task.CompletedTask;
    }
}
