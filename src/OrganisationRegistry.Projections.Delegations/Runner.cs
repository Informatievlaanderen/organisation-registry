namespace OrganisationRegistry.Projections.Delegations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Configuration;
    using Info;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Projections;
    using SqlServer.Infrastructure;
    using SqlServer.ProjectionState;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Infrastructure.Events;

    public class Runner
    {
        public const string DelegationsRunnerProjectionName = "DelegationsRunner";

        private readonly ILogger<Runner> _logger;
        private readonly TogglesConfiguration _togglesConfiguration;
        private readonly DelegationsRunnerConfiguration _delegationsRunnerConfiguration;
        private readonly IEventStore _store;
        private readonly IProjectionStates _projectionStates;
        private readonly IEventPublisher _bus;
        private readonly List<Type> _eventHandlers;

        private const int BatchSize = 5000;

        public Runner(
            ILogger<Runner> logger,
            IOptions<TogglesConfiguration> togglesConfigurationOptions,
            IOptions<DelegationsRunnerConfiguration> delegationsRunnerConfiguration,
            IEventStore store,
            IProjectionStates projectionStates,
            IEventPublisher bus)
        {
            _logger = logger;
            _delegationsRunnerConfiguration = delegationsRunnerConfiguration.Value;
            _togglesConfiguration = togglesConfigurationOptions.Value;
            _store = store;
            _projectionStates = projectionStates;
            _bus = bus;
            _eventHandlers = new List<Type>
            {
                typeof(MemoryCachesMaintainer),
                typeof(DelegationListProjection),
                typeof(PersonMandateListProjection)
            };
        }

        public bool Run()
        {
            _logger.LogInformation(ProgramInformation.Build(_delegationsRunnerConfiguration, _togglesConfiguration));

            if (!_togglesConfiguration.DelegationsRunnerAvailable)
                return false;

            var lastProcessedEventNumber = _projectionStates.GetLastProcessedEventNumber(DelegationsRunnerProjectionName);
            InitialiseProjection(lastProcessedEventNumber);

            var eventsBeingListenedTo =
                _eventHandlers
                    .SelectMany(handler =>
                        handler.GetTypeInfo()
                            .ImplementedInterfaces
                            .SelectMany(@interface =>
                                @interface.GenericTypeArguments))
                    .Distinct()
                    .ToList();

            var envelopes = _store.GetEventEnvelopesAfter(lastProcessedEventNumber, BatchSize, eventsBeingListenedTo.ToArray()).ToList();

            LogEnvelopeCount(envelopes);

            var newLastProcessedEventNumber = new int?();
            try
            {
                envelopes.ForEach(envelope =>
                    newLastProcessedEventNumber = ProcessEnvelope(envelope));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(0, ex, "An exception occurred while handling envelopes.");
                throw;
            }
            finally
            {
                UpdateProjectionState(newLastProcessedEventNumber);
            }

            return true;
        }

        private void InitialiseProjection(int lastProcessedEventNumber)
        {
            if (lastProcessedEventNumber != -1)
                return;

            _logger.LogInformation("[{ProjectionName}] First run, initialising projections!", DelegationsRunnerProjectionName);
            ProcessEnvelope(new InitialiseProjection(typeof(DelegationListProjection).FullName).ToTypedEnvelope());
        }

        private void UpdateProjectionState(int? newLastProcessedEventNumber)
        {
            if (!newLastProcessedEventNumber.HasValue)
                return;

            _logger.LogInformation("Processed up until envelope #{LastProcessedEnvelopeNumber}, writing number to db...", newLastProcessedEventNumber);
            _projectionStates.UpdateProjectionState(DelegationsRunnerProjectionName, newLastProcessedEventNumber.Value);
        }

        private int? ProcessEnvelope(IEnvelope envelope)
        {
            try
            {
                _bus.Publish(null, null, (dynamic) envelope);
                return envelope.Number;
            }
            catch
            {
                _logger.LogCritical(
                    "An exception occurred while processing envelope #{EnvelopeNumber}:{@EnvelopeJson}",
                    envelope.Number,
                    envelope);

                throw;
            }
        }

        private void LogEnvelopeCount(IReadOnlyCollection<IEnvelope> envelopes)
        {
            _logger.LogInformation("Found {NumberOfEnvelopes} envelopes to process.", envelopes.Count);
            //_telemetryClient.TrackMetric("DelegationsRunner::EnvelopesToProcess", envelopes.Count);

            if (envelopes.Count > 0)
            {
                var firstEnvelope = envelopes.First();
                var lastEnvelope = envelopes.Last();
                _logger.LogInformation("Starting at #{FirstEnvelopeNumber} to #{LastEnvelopeNumber}.", firstEnvelope.Number, lastEnvelope.Number);
            }
        }
    }
}
