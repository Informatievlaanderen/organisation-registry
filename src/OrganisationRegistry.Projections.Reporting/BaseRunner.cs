namespace OrganisationRegistry.Projections.Reporting
{
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using SqlServer.ProjectionState;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using OrganisationRegistry.Infrastructure.Events;

    public class BaseRunner
    {
        private const int BatchSize = 50000;

        public string ProjectionName { get; }
        public Type[] EventHandlers { get; }
        public Type[] ReactionHandlers { get; }

        private readonly ILogger<BaseRunner> _logger;
        private readonly IEventStore _store;
        private readonly IProjectionStates _projectionStates;
        private readonly IEventPublisher _bus;
        private readonly string _dbProjectionsProjectionName;
        private readonly string _projectionFullName;

        protected BaseRunner(
            ILogger<BaseRunner> logger,
            IEventStore store,
            IProjectionStates projectionStates,
            IEventPublisher bus,
            string dbProjectionsProjectionName,
            string projectionFullName,
            string projectionName,
            Type[] eventHandlers,
            Type[] reactionHandlers)
        {
            ProjectionName = projectionName;
            EventHandlers = eventHandlers;
            ReactionHandlers = reactionHandlers;

            _logger = logger;
            _store = store;
            _projectionStates = projectionStates;
            _bus = bus;
            _dbProjectionsProjectionName = dbProjectionsProjectionName;
            _projectionFullName = projectionFullName;
        }

        public void Run()
        {
            var lastProcessedEventNumber = _projectionStates.GetLastProcessedEventNumber(_dbProjectionsProjectionName);
            InitialiseProjection(lastProcessedEventNumber);

            var eventsBeingListenedTo =
                EventHandlers
                    .SelectMany(handler => handler
                        .GetTypeInfo()
                        .ImplementedInterfaces
                        .SelectMany(@interface => @interface.GenericTypeArguments))
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
                _logger.LogCritical(0, ex, "[{ProjectionName}] An exception occurred while handling envelopes.", ProjectionName);

                throw;
            }
            finally
            {
                UpdateProjectionState(newLastProcessedEventNumber);
            }
        }

        private void InitialiseProjection(int lastProcessedEventNumber)
        {
            if (lastProcessedEventNumber != -1)
                return;

            _logger.LogInformation("[{ProjectionName}] First run, initialising projections!", ProjectionName);

            ProcessEnvelope(new InitialiseProjection(_projectionFullName).ToTypedEnvelope());
        }

        private void UpdateProjectionState(int? newLastProcessedEventNumber)
        {
            if (!newLastProcessedEventNumber.HasValue)
                return;

            _logger.LogInformation("[{ProjectionName}] Processed up until envelope #{LastProcessedEnvelopeNumber}, writing number to db...", ProjectionName, newLastProcessedEventNumber);

            _projectionStates.UpdateProjectionState(_dbProjectionsProjectionName, newLastProcessedEventNumber.Value);
        }

        private int? ProcessEnvelope(IEnvelope envelope)
        {
            try
            {
                _bus.Publish(null, null, (dynamic)envelope);
                return envelope.Number;
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

            //_telemetryClient.TrackMetric($"ReportingProjections::{ProjectionName}::EnvelopesToProcess", envelopes.Count);

            if (envelopes.Count > 0)
                _logger.LogInformation("[{ProjectionName}] Starting at #{FirstEnvelopeNumber} to #{LastEnvelopeNumber}.", ProjectionName, envelopes.First().Number, envelopes.Last().Number);
        }
    }
}
