namespace OrganisationRegistry.ElasticSearch.Projections.Cache;

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using App.Metrics;
using Infrastructure;
using Metrics;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Config;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer;
using SqlServer.ProjectionState;

public class CacheRunner
{
    public static readonly Type[] EventHandlers =
    {
        typeof(OrganisationCache),
        typeof(BodySeatCache),
        typeof(BodyCache),
        typeof(ContactTypeCache),
    };

    public static string ProjectionName => "ElasticSearchCache";

    private readonly ILogger<CacheRunner> _logger;
    private readonly IEventStore _store;
    private readonly IContextFactory _contextFactory;
    private readonly IProjectionStates _projectionStates;
    private readonly IEventPublisher _bus;

    private readonly EnvelopeMetrics _metrics;

    public CacheRunner(ILogger<CacheRunner> logger,
        IEventStore store,
        IContextFactory contextFactory,
        IProjectionStates projectionStates,
        IEventPublisher bus, BusRegistrar busRegistrar,
        IMetricsRoot metrics)
    {
        _logger = logger;
        _store = store;
        _contextFactory = contextFactory;
        _projectionStates = projectionStates;
        _bus = bus;
        busRegistrar.RegisterEventHandlers(EventHandlers);

        _metrics = new EnvelopeMetrics(metrics, ProjectionName);
    }

    public async Task Run()
    {
        var eventsBeingListenedTo =
            EventHandlers
                .SelectMany(handler => handler
                    .GetTypeInfo()
                    .ImplementedInterfaces
                    .SelectMany(@interface => @interface.GenericTypeArguments))
                .Distinct()
                .ToList();

        await using var context = _contextFactory.Create();

        var lastEvent = _store.GetLastEvent();
        var lastProcessedEventNumber = await _projectionStates.GetLastProcessedEventNumber(ProjectionName);
        await InitialiseProjection(lastProcessedEventNumber);

        var previousLastProcessedEventNumber = (int?)null;

        while (lastEvent > lastProcessedEventNumber && lastProcessedEventNumber != previousLastProcessedEventNumber)
        {
            previousLastProcessedEventNumber = lastProcessedEventNumber;
            var envelopes = _store.GetEventEnvelopesAfter(lastProcessedEventNumber, 2500, eventsBeingListenedTo.ToArray()).ToList();

            _metrics.CountEnvelopes(envelopes);
            _metrics.MeterEnvelopes(envelopes);

            var newLastProcessedEventNumber = new int?();
            try
            {
                foreach (var envelope in envelopes)
                {
                    newLastProcessedEventNumber = await ProcessEnvelope(envelope);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(0, ex, "[{ProjectionName}] An exception occurred while handling envelopes", ProjectionName);
            }
            finally
            {
                await UpdateProjectionState(newLastProcessedEventNumber);
            }

            lastProcessedEventNumber = await _projectionStates.GetLastProcessedEventNumber(ProjectionName);
        }
    }

    private async Task InitialiseProjection(int lastProcessedEventNumber)
    {
        if (lastProcessedEventNumber != -1)
            return;

        _logger.LogInformation("[{ProjectionName}] First run, initialising projections!", ProjectionName);
        await ProcessEnvelope(new InitialiseProjection(ProjectionName).ToTypedEnvelope());
    }

    private async Task UpdateProjectionState(int? newLastProcessedEventNumber)
    {
        if (!newLastProcessedEventNumber.HasValue)
            return;

        _logger.LogInformation("[{ProjectionName}] Processed up until envelope #{LastProcessedEnvelopeNumber}, writing number to db...", ProjectionName, newLastProcessedEventNumber);
        await _projectionStates.UpdateProjectionState(ProjectionName, newLastProcessedEventNumber.Value);
    }

    private async Task<int?> ProcessEnvelope(IEnvelope envelope)
    {
        try
        {
            await _bus.Publish(null, null, (dynamic)envelope);
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
}