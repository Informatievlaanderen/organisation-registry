namespace OrganisationRegistry.Infrastructure.Domain
{
    using System;
    using System.Collections.Generic;
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

            logger.LogTrace("Creating Session.");
        }

        public void Add<T>(T aggregate) where T : AggregateRoot
        {
            if (!IsTracked(aggregate.Id))
                _trackedAggregates.Add(aggregate.Id, new AggregateDescriptor { Aggregate = aggregate, Version = aggregate.Version });

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

        public void Commit()
        {
            foreach (var descriptor in _trackedAggregates.Values)
                _repository.Save(descriptor.Aggregate, descriptor.Version);

            Reset();
        }

        public void Reset()
            => _trackedAggregates.Clear();

        private class AggregateDescriptor
        {
            public AggregateRoot Aggregate { get; set; }
            public int Version { get; set; }
        }
    }
}
