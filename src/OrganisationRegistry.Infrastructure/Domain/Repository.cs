namespace OrganisationRegistry.Infrastructure.Domain;

using System;
using Events;
using Exception;
using Factories;
using System.Linq;
using System.Threading.Tasks;
using Authorization;
using EventStore;
using Microsoft.Extensions.Logging;

// Scoped as SingleInstance()
public class Repository : IRepository
{
    private readonly IEventStore _eventStore;

    public Repository(
        ILogger<Repository> logger,
        IEventStore eventStore)
    {
        _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));

        logger.LogTrace("Creating Repository");
    }

    public async Task Save<T>(T aggregate, IUser user, int? expectedVersion = null) where T : AggregateRoot
    {
        if (expectedVersion != null && _eventStore.Get<T>(aggregate.Id, expectedVersion.Value).Any())
            throw new ConcurrencyException(aggregate.Id, expectedVersion.Value);

        var changes = aggregate.FlushUncommitedChanges();
        await _eventStore.Save<T>(changes, user);
    }

    public T Get<T>(Guid aggregateId) where T : AggregateRoot
    {
        var events = _eventStore.Get<T>(aggregateId, FromVersion.Start).ToList();
        if (!events.Any())
            throw new AggregateNotFoundException(typeof(T), aggregateId);

        var aggregate = AggregateFactory.CreateAggregate<T>();
        aggregate.LoadFromHistory(events);
        return aggregate;
    }
}
