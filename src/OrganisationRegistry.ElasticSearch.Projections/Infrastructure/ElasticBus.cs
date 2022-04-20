namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading.Tasks;
    using Change;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Infrastructure.Messages;

    public class ElasticBus
    {
        private readonly ILogger<ElasticBus> _logger;
        private readonly Dictionary<Type, List<Func<DbConnection, DbTransaction, IMessage, Task<IElasticChange>>>> _eventRoutes = new();

        public ElasticBus(ILogger<ElasticBus> logger)
        {
            _logger = logger;
            _logger.LogTrace("Creating ElasticBus");
        }

        public void RegisterEventHandler<T>(Func<DbConnection, DbTransaction, IEnvelope<T>, Task<IElasticChange>> handler) where T : IEvent<T>
        {
            if (!_eventRoutes.TryGetValue(typeof(T), out var handlers))
            {
                handlers = new List<Func<DbConnection, DbTransaction, IMessage, Task<IElasticChange>>>();
                _eventRoutes.Add(typeof(T), handlers);
            }

            handlers.Add(async (dbConnection, dbTransaction, @event) => await handler(dbConnection, dbTransaction, (IEnvelope<T>)@event));
        }

        public async Task<IEnumerable<IElasticChange>> Publish<T>(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<T> envelope) where T : IEvent<T>
        {
            if (!_eventRoutes.TryGetValue(envelope.Body.GetType(), out var handlers))
                handlers = new List<Func<DbConnection, DbTransaction, IMessage, Task<IElasticChange>>>();

            _logger.LogDebug("Publishing event {@Event} to {NumberOfEventHandlers} event handlers", envelope, handlers.Count);

            var changes = new List<IElasticChange>();

            foreach (var handler in handlers)
                changes.Add(await handler(dbConnection, dbTransaction, envelope));

            return changes;
        }
    }
}
