namespace OrganisationRegistry.ElasticSearch.Projections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Client;
    using Configuration;
    using Infrastructure;
    using Infrastructure.Change;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nest;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer.ProjectionState;

    public abstract class BaseRunner<T> where T: class, IDocument, new()
    {
        public string ProjectionName { get; }
        public Type[] EventHandlers { get; }

        private readonly string _elasticSearchProjectionsProjectionName;
        private readonly string _projectionFullName;
        private readonly Elastic _elastic;

        private readonly int _batchSize;
        private readonly ILogger<BaseRunner<T>> _logger;
        private readonly IEventStore _store;
        private readonly IProjectionStates _projectionStates;
        private readonly ElasticBus _bus;

        protected BaseRunner(
            ILogger<BaseRunner<T>> logger,
            IOptions<ElasticSearchConfiguration> configuration,
            IEventStore store,
            IProjectionStates projectionStates,
            string elasticSearchProjectionsProjectionName,
            string projectionFullName,
            string projectionName,
            Type[] eventHandlers,
            Elastic elastic,
            ElasticBus bus)
        {
            _logger = logger;
            _store = store;
            _projectionStates = projectionStates;
            _bus = bus;

            _batchSize = configuration.Value.BatchSize;
            _elasticSearchProjectionsProjectionName = elasticSearchProjectionsProjectionName;
            _projectionFullName = projectionFullName;
            _elastic = elastic;

            ProjectionName = projectionName;
            EventHandlers = eventHandlers;
        }

        public async Task Run()
        {
            var lastProcessedEventNumber = await _projectionStates.GetLastProcessedEventNumber(_elasticSearchProjectionsProjectionName);
            await InitialiseProjection(lastProcessedEventNumber);

            var eventsBeingListenedTo =
                EventHandlers
                    .SelectMany(handler => handler
                        .GetTypeInfo()
                        .ImplementedInterfaces
                        .SelectMany(@interface => @interface.GenericTypeArguments))
                    .Distinct()
                    .ToList();

            var envelopes = _store
                .GetEventEnvelopesAfter(lastProcessedEventNumber, _batchSize, eventsBeingListenedTo.ToArray())
                .ToList();

            LogEnvelopeCount(envelopes);

            try
            {
                var allChanges = new List<ElasticChanges>();
                foreach (var envelope in envelopes)
                {
                    var changes = await ProcessEnvelope(envelope);
                    allChanges.Add(changes);
                }

                var documentCache = new Dictionary<Guid, T>();

                foreach (var changeSet in allChanges)
                {
                    int? newLastProcessedEventNumber = changeSet.EnvelopeNumber;

                    foreach (var changeSetChange in changeSet.Changes)
                    {
                        switch (changeSetChange)
                        {
                            case ElasticPerDocumentChange<T> perDocumentChange:
                            {
                                foreach (var documentChange in perDocumentChange.Changes)
                                {
                                    var document = new T();

                                    if (!documentCache.ContainsKey(documentChange.Key))
                                    {
                                        if ((await _elastic.TryGetAsync(() => _elastic.WriteClient.DocumentExistsAsync<T>(documentChange.Key))).Exists)
                                        {
                                            document = (await _elastic.TryGetAsync(() =>
                                                    _elastic.WriteClient.GetAsync<T>(documentChange.Key)))
                                                .ThrowOnFailure()
                                                .Source;
                                        }
                                        documentCache.Add(documentChange.Key, document);
                                    }
                                    else
                                    {
                                        document = documentCache[documentChange.Key];
                                    }

                                    documentChange.Value(document);
                                }

                                break;
                            }
                            case ElasticMassChange massChange:
                            {
                                await FlushDocuments(documentCache, newLastProcessedEventNumber);
                                await massChange.Change(_elastic);
                                (await _elastic.TryGetAsync(async () =>
                                        await _elastic.WriteClient.Indices.RefreshAsync(Indices.Index<T>())))
                                    .ThrowOnFailure();
                                break;
                            }
                        }
                    }
                    await FlushDocuments(documentCache, newLastProcessedEventNumber);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(0, ex, "[{ProjectionName}] An exception occurred while handling envelopes.",
                    ProjectionName);
            }
        }

        private async Task FlushDocuments(Dictionary<Guid, T> documentCache, int? newLastProcessedEventNumber)
        {
            if (documentCache.Any())
            {
                (await _elastic.TryGetAsync(async () =>
                        await _elastic.WriteClient.IndexManyAsync(documentCache.Values)))
                    .ThrowOnFailure();
                await UpdateProjectionState(newLastProcessedEventNumber);
                documentCache.Clear();
            }
        }

        private async Task InitialiseProjection(int lastProcessedEventNumber)
        {
            if (lastProcessedEventNumber != -1)
                return;

            _logger.LogInformation("[{ProjectionName}] First run, initialising projections!", ProjectionName);
            await ProcessEnvelope(new InitialiseProjection(_projectionFullName).ToTypedEnvelope());
        }

        private async Task UpdateProjectionState(int? newLastProcessedEventNumber)
        {
            if (!newLastProcessedEventNumber.HasValue)
                return;

            _logger.LogInformation("[{ProjectionName}] Processed up until envelope #{LastProcessedEnvelopeNumber}, writing number to db...", ProjectionName, newLastProcessedEventNumber);
            await _projectionStates.UpdateProjectionState(_elasticSearchProjectionsProjectionName, newLastProcessedEventNumber.Value);
        }

        private async Task<ElasticChanges> ProcessEnvelope(IEnvelope envelope)
        {
            try
            {
                var changes = await _bus.Publish(null, null, (dynamic)envelope);
                return new ElasticChanges(envelope.Number, changes);
            }
            catch
            {
                _logger.LogCritical(
                    "[{ProjectionName}] An exception occurred while processing envelope #{EnvelopeNumber}:{@EnvelopeJson}",
                    ProjectionName,
                    envelope.Number,
                    envelope);

                throw;
            }
        }

        private void LogEnvelopeCount(IReadOnlyCollection<IEnvelope> envelopes)
        {
            _logger.LogInformation("[{ProjectionName}] Found {NumberOfEnvelopes} envelopes to process.", ProjectionName, envelopes.Count);

            if (envelopes.Count > 0)
                _logger.LogInformation("[{ProjectionName}] Starting at #{FirstEnvelopeNumber} to #{LastEnvelopeNumber}.", ProjectionName, envelopes.First().Number, envelopes.Last().Number);
        }
    }
}
