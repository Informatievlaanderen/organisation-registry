namespace OrganisationRegistry.ElasticSearch.Projections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using App.Metrics;
    using App.Metrics.Timer;
    using Bodies;
    using Client;
    using Configuration;
    using Infrastructure;
    using Infrastructure.Change;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nest;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer.ProjectionState;
    using TimeUnit = App.Metrics.TimeUnit;

    public abstract class BaseRunner<T> where T: class, IDocument, new()
    {
        public string ProjectionName { get; }
        public Type[] EventHandlers { get; }

        private readonly string _elasticSearchProjectionsProjectionName;
        private readonly string _projectionFullName;
        private readonly Elastic _elastic;
        private readonly IMetricsRoot _metrics;

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
            ElasticBus bus,
            IMetricsRoot metrics)
        {
            _logger = logger;
            _store = store;
            _projectionStates = projectionStates;
            _bus = bus;

            _batchSize = configuration.Value.BatchSize;
            _elasticSearchProjectionsProjectionName = elasticSearchProjectionsProjectionName;
            _projectionFullName = projectionFullName;
            _elastic = elastic;
            _metrics = metrics;

            ProjectionName = projectionName;
            EventHandlers = eventHandlers;
        }

        public async Task Run()
        {
            var changeDocumentTimer = new TimerOptions
            {
                Name = "Change Document Timer",
                MeasurementUnit = Unit.Calls,
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds
            };

            var getEventsTimer = new TimerOptions
            {
                Name = "Get Events Timer",
                MeasurementUnit = Unit.Calls,
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds
            };

            var lastProcessedEventNumber = _projectionStates.GetLastProcessedEventNumber(_elasticSearchProjectionsProjectionName);
            await InitialiseProjection(lastProcessedEventNumber);

            var eventsBeingListenedTo =
                EventHandlers
                    .SelectMany(handler => handler
                        .GetTypeInfo()
                        .ImplementedInterfaces
                        .SelectMany(@interface => @interface.GenericTypeArguments))
                    .Distinct()
                    .ToList();

            var envelopes = new List<IEnvelope>();

            _metrics.Measure.Timer.Time(getEventsTimer, () =>
            {
                envelopes = _store
                    .GetEventEnvelopesAfter(lastProcessedEventNumber, _batchSize, eventsBeingListenedTo.ToArray())
                    .ToList();
            });

            LogEnvelopeCount(envelopes);

            var newLastProcessedEventNumber = new int?();
            try
            {
                var allChanges = new List<ElasticChanges>();
                foreach (var envelope in envelopes)
                {
                    var changes = await ProcessEnvelope(envelope);
                    allChanges.Add(changes);
                    newLastProcessedEventNumber = changes.EnvelopeNumber;
                }

                var changesByEnvelopeNumber = allChanges
                    .OrderBy(x => x.EnvelopeNumber)
                    .SelectMany(x => x.Changes)
                    .ToList();

                var changesPerDocument = changesByEnvelopeNumber
                    .OfType<ElasticPerDocumentChange<T>>()
                    .SelectMany(change => change.Changes)
                    .GroupBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Value));

                var massUpdates = changesByEnvelopeNumber
                    .OfType<ElasticMassChange>();

                var documents = new Dictionary<Guid, T>();

                foreach (var changeSet in changesPerDocument)
                {
                    var document = new T();

                    if (!documents.ContainsKey(changeSet.Key))
                    {
                        if ((await _elastic.TryGetAsync(() => _elastic.WriteClient.DocumentExistsAsync<T>(changeSet.Key))).Exists)
                        {
                            document = (await _elastic.TryGetAsync(() =>
                                    _elastic.WriteClient.GetAsync<T>(changeSet.Key)))
                                .ThrowOnFailure()
                                .Source;
                        }
                        documents.Add(changeSet.Key, document);
                    }
                    else
                    {
                        document = documents[changeSet.Key];
                    }

                    foreach (var change in changeSet.Value)
                    {
                        _metrics.Measure.Timer.Time(changeDocumentTimer, () => change(document));
                    }
                }

                if (documents.Any())
                {
                    (await _elastic.TryGetAsync(async () =>
                            await _elastic.WriteClient.IndexManyAsync<T>(documents.Values)))
                        .ThrowOnFailure();
                }

                foreach (var massUpdate in massUpdates)
                {
                    await massUpdate.Change(_elastic);
                }

                UpdateProjectionState(newLastProcessedEventNumber);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(0, ex, "[{ProjectionName}] An exception occurred while handling envelopes.",
                    ProjectionName);
            }
            finally
            {
                await Task.WhenAll(_metrics.ReportRunner.RunAllAsync());
            }
        }

        private async Task InitialiseProjection(int lastProcessedEventNumber)
        {
            if (lastProcessedEventNumber != -1)
                return;

            _logger.LogInformation("[{ProjectionName}] First run, initialising projections!", ProjectionName);
            await ProcessEnvelope(new InitialiseProjection(_projectionFullName).ToTypedEnvelope());
        }

        private void UpdateProjectionState(int? newLastProcessedEventNumber)
        {
            if (!newLastProcessedEventNumber.HasValue)
                return;

            _logger.LogInformation("[{ProjectionName}] Processed up until envelope #{LastProcessedEnvelopeNumber}, writing number to db...", ProjectionName, newLastProcessedEventNumber);
            _projectionStates.UpdateProjectionState(_elasticSearchProjectionsProjectionName, newLastProcessedEventNumber.Value);
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
