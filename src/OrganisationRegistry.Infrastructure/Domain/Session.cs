namespace OrganisationRegistry.Infrastructure.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authorization;
using Exception;
using Microsoft.Extensions.Logging;

// Scoped as InstancePerLifetimeScope()
public class Session : ISession
{
    private readonly IRepository _repository;
    private readonly Dictionary<Guid, AggregateDescriptor> _trackedAggregates;

    public Session(
        ILogger<Session> logger,
        IRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _trackedAggregates = new Dictionary<Guid, AggregateDescriptor>();

        logger.LogTrace("Creating Session");
    }

    public void Add<T>(T aggregate) where T : AggregateRoot
    {
        if (!IsTracked(aggregate.Id))
            _trackedAggregates.Add(aggregate.Id, new AggregateDescriptor(aggregate, aggregate.Version));

        // ReSharper disable once PossibleUnintendedReferenceComparison
        else if (_trackedAggregates[aggregate.Id].Aggregate != aggregate)
            throw new ConcurrencyException(aggregate.Id);
    }

    public T Get<T>(Guid id, int? expectedVersion = null) where T : AggregateRoot
    {
        if (IsTracked(id))
        {
            var trackedAggregate = (T)_trackedAggregates[id].Aggregate;
            if (expectedVersion != null && trackedAggregate.Version != expectedVersion)
                throw new ConcurrencyException(trackedAggregate.Id, expectedVersion.GetValueOrDefault(-1), trackedAggregate.Version);

            return trackedAggregate;
        }

        var aggregate = _repository.Get<T>(id);
        if (expectedVersion != null && aggregate.Version != expectedVersion)
            throw new ConcurrencyException(id, expectedVersion.GetValueOrDefault(-1), aggregate.Version);

        Add(aggregate);

        return aggregate;
    }

    private bool IsTracked(Guid id)
        => _trackedAggregates.ContainsKey(id);

    public async Task Commit(IUser user)
    {
        foreach (var descriptor in _trackedAggregates.Values)
            await _repository.Save(descriptor.Aggregate, user, descriptor.Version);

        Reset();
    }

    public async Task CommitAllInOneTransaction(IUser user)
    {
        var aggregates = _trackedAggregates.Values.Select(descriptor => descriptor.Aggregate);

        await _repository.Save(aggregates, user);

        Reset();
    }

    public void Reset()
        => _trackedAggregates.Clear();

    private record AggregateDescriptor(AggregateRoot Aggregate, int Version);
}
