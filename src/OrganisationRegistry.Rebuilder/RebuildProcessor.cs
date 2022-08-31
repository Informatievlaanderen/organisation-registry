namespace OrganisationRegistry.Rebuilder;

using System;
using System.Data;
using System.Threading.Tasks;
using Infrastructure.Configuration;
using Infrastructure.Events;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlServer.ProjectionState;

public class RebuildProcessor
{
    private const string ProjectionName = "OrganisationRegistry.Rebuilder";
    private readonly ILogger<RebuildProcessor> _logger;
    private readonly IEventStore _store;
    private readonly IProjectionStates _projectionStates;
    private readonly IOptions<InfrastructureConfigurationSection> _infrastructureOption;
    private readonly IEventPublisher _publisher;

    public RebuildProcessor(
        ILogger<RebuildProcessor> logger,
        IEventStore store,
        IProjectionStates projectionStates,
        IOptions<InfrastructureConfigurationSection> infrastructureOption,
        IEventPublisher publisher)
    {
        _logger = logger;
        _store = store;
        _projectionStates = projectionStates;
        _infrastructureOption = infrastructureOption;
        _publisher = publisher;
    }

    public async Task Run()
    {
        var number = -1;
        try{
            if (!await _projectionStates.Exists(ProjectionName).ConfigureAwait(false))
            {
                _logger.LogWarning("No record found in Backoffice.ProjectionStateList table " +
                                   "you might need to add a record for name='{ProjectionName}'", ProjectionName);
                return;
            }

            _logger.LogInformation("Found desired projection state");

            number = await _projectionStates.GetLastProcessedEventNumber(ProjectionName)
                .ConfigureAwait(false);
            var lastNumber = _store.GetLastEvent();

            while (number != lastNumber)
            {
                _logger.LogInformation("Catching up from {Number}", number);
                var events = _store.GetEventEnvelopesAfter(number, 5000);

                var connection = new SqlConnection(_infrastructureOption.Value.EventStoreConnectionString);
                await using var _ = connection.ConfigureAwait(false);
                await connection.OpenAsync().ConfigureAwait(false);
                var tx = await connection.BeginTransactionAsync(IsolationLevel.Serializable).ConfigureAwait(false);
                await using var __ = tx.ConfigureAwait(false);
                foreach (var @event in events)
                {
                    await _publisher.Publish(connection, tx, (dynamic)@event);
                    number = @event.Number;
                }

                await _projectionStates.UpdateProjectionState(ProjectionName, number, connection, tx)
                    .ConfigureAwait(false);

                lastNumber = await _projectionStates.GetLastProcessedEventNumber(ProjectionName)
                    .ConfigureAwait(false);

                await tx.CommitAsync().ConfigureAwait(false);
            }
            await _projectionStates.Remove(ProjectionName).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(0, ex,
                "An exception occurred while handling envelope number {Number}", number);
        }
    }
}
