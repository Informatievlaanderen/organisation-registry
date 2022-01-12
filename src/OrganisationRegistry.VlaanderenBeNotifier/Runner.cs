namespace OrganisationRegistry.VlaanderenBeNotifier
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Info;
    using Infrastructure.Configuration;
    using Infrastructure.Events;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using SqlServer.Configuration;
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
        private readonly SqlServerConfiguration _sqlServerConfiguration;

        public Runner(
            ILogger<Runner> logger,
            IOptions<TogglesConfiguration> togglesConfigurationOptions,
            IOptions<VlaanderenBeNotifierConfiguration> vlaanderenBeNotifierConfigurationOptions,
            IEventStore store,
            IProjectionStates projectionStates,
            IEventPublisher bus,
            IOptions<SqlServerConfiguration> sqlServerConfiguration)
        {
            _logger = logger;
            _vlaanderenBeNotifierConfiguration = vlaanderenBeNotifierConfigurationOptions.Value;
            _togglesConfiguration = togglesConfigurationOptions.Value;
            _store = store;
            _projectionStates = projectionStates;
            _bus = bus;
            _sqlServerConfiguration = sqlServerConfiguration.Value;
        }

        public async Task<bool> Run()
        {
            _logger.LogInformation(ProgramInformation.Build(_vlaanderenBeNotifierConfiguration));

            if (!_togglesConfiguration.VlaanderenBeNotifierAvailable)
                return false;

            var lastProcessedEventNumber = await _projectionStates.GetLastProcessedEventNumber(VlaanderenbeNotifierProjectionName);
            var envelopes = _store.GetEventEnvelopesAfter(lastProcessedEventNumber, 5000).ToList();

            LogEnvelopeCount(envelopes);

            var newLastProcessedEventNumber = new int?();
            try
            {
                var db = new SqlConnection(_sqlServerConfiguration.ConnectionString);
                await db.OpenAsync();
                foreach (var envelope in envelopes)
                {
                    var tx = await db.BeginTransactionAsync(IsolationLevel.Serializable);

                    newLastProcessedEventNumber = await ProcessEnvelope(envelope, db, tx);
                    await UpdateProjectionState(newLastProcessedEventNumber, db, tx);

                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(0, ex, "An exception occurred while handling envelopes.");
                throw;
            }

            return true;
        }

        private async Task UpdateProjectionState(int? newLastProcessedEventNumber, SqlConnection sqlConnection,
            DbTransaction dbTransaction)
        {
            if (!newLastProcessedEventNumber.HasValue)
                return;

            _logger.LogInformation("Processed up until envelope #{LastProcessedEnvelopeNumber}, writing number to db...", newLastProcessedEventNumber);
            await _projectionStates.UpdateProjectionState(VlaanderenbeNotifierProjectionName,
                newLastProcessedEventNumber.Value,
                sqlConnection, dbTransaction);
        }

        private async Task<int?> ProcessEnvelope(IEnvelope envelope, SqlConnection db, DbTransaction tx)
        {
            try
            {
                await _bus.Publish(db, tx, (dynamic)envelope);
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
