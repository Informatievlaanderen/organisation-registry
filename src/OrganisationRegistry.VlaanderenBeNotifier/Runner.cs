namespace OrganisationRegistry.VlaanderenBeNotifier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Info;
    using Infrastructure.Configuration;
    using Infrastructure.Events;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using SqlServer.ProjectionState;

    public class Runner
    {
        public const string VlaanderenbeNotifierProjectionName = "VlaanderenBeNotifier";

        private readonly ILogger<Runner> _logger;
        private readonly TogglesConfiguration _togglesConfiguration;
        private readonly VlaanderenBeNotifierConfiguration _vlaanderenBeNotifierConfiguration;
        private readonly IEventStore _store;
        private readonly IProjectionStates _projectionStates;
        private readonly IEventPublisher _bus;

        public Runner(
            ILogger<Runner> logger,
            IOptions<TogglesConfiguration> togglesConfigurationOptions,
            IOptions<VlaanderenBeNotifierConfiguration> vlaanderenBeNotifierConfigurationOptions,
            IEventStore store,
            IProjectionStates projectionStates,
            IEventPublisher bus)
        {
            _logger = logger;
            _vlaanderenBeNotifierConfiguration = vlaanderenBeNotifierConfigurationOptions.Value;
            _togglesConfiguration = togglesConfigurationOptions.Value;
            _store = store;
            _projectionStates = projectionStates;
            _bus = bus;
        }

        public bool Run()
        {
            _logger.LogInformation(ProgramInformation.Build(_vlaanderenBeNotifierConfiguration));

            if (!_togglesConfiguration.VlaanderenBeNotifierAvailable)
                return false;

            var lastProcessedEventNumber = _projectionStates.GetLastProcessedEventNumber(VlaanderenbeNotifierProjectionName);
            var envelopes = _store.GetEventEnvelopesAfter(lastProcessedEventNumber).ToList();

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

        private void UpdateProjectionState(int? newLastProcessedEventNumber)
        {
            if (!newLastProcessedEventNumber.HasValue)
                return;

            _logger.LogInformation("Processed up until envelope #{LastProcessedEnvelopeNumber}, writing number to db...", newLastProcessedEventNumber);
            _projectionStates.UpdateProjectionState(VlaanderenbeNotifierProjectionName, newLastProcessedEventNumber.Value);
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
            //_telemetryClient.TrackMetric("VlaanderenBeNotifier::EnvelopesToProcess", envelopes.Count);

            if (envelopes.Count > 0)
                _logger.LogInformation("Starting at #{FirstEnvelopeNumber} to #{LastEnvelopeNumber}.", envelopes.First().Number, envelopes.Last().Number);
        }
    }
}
