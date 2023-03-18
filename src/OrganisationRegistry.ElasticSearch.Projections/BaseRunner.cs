namespace OrganisationRegistry.ElasticSearch.Projections;

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Client;
using Configuration;
using Infrastructure;
using Infrastructure.Change;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using Osc;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer;
using SqlServer.ElasticSearchProjections;
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
    private readonly IContextFactory _contextFactory;
    private readonly OpenTelemetryMetrics.ElasticSearchProjections _metrics;

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
        IContextFactory contextFactory)
    {
        _logger = logger;
        _store = store;
        _projectionStates = projectionStates;
        _bus = bus;
        _contextFactory = contextFactory;

        _batchSize = configuration.Value.BatchSize;
        _elasticSearchProjectionsProjectionName = elasticSearchProjectionsProjectionName;
        _projectionFullName = projectionFullName;
        _elastic = elastic;

        ProjectionName = projectionName;
        EventHandlers = eventHandlers;

        _metrics = new OpenTelemetryMetrics.ElasticSearchProjections(projectionName);
    }

    public async Task Run()
    {
        var maxEventNumberToProcess = _store.GetLastEvent();
        _metrics.MaxEventNumberToProcess = maxEventNumberToProcess;

        var lastProcessedEventNumber = await _projectionStates.GetLastProcessedEventNumber(_elasticSearchProjectionsProjectionName);
        _metrics.NumberOfEnvelopesBehind = maxEventNumberToProcess - lastProcessedEventNumber;

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

        int? newLastProcessedEventNumber = null;
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
                newLastProcessedEventNumber = changeSet.EnvelopeNumber;
                foreach (var changeSetChange in changeSet.Changes)
                {
                    await ProcessChange(changeSetChange, documentCache, newLastProcessedEventNumber);
                }
            }
            await FlushDocuments(documentCache);
            await UpdateProjectionState(newLastProcessedEventNumber);
            _metrics.NumberOfEnvelopesHandled.Record(envelopes.Count);
        }
        catch (ElasticsearchOrganisationNotFoundException organisationNotFoundException)
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            organisationRegistryContext.OrganisationsToRebuild.Add(new OrganisationToRebuild
            {
                OrganisationId = Guid.Parse(organisationNotFoundException.OrganisationId)
            });
            await organisationRegistryContext.SaveChangesAsync();
            _logger.LogWarning(
                0,
                organisationNotFoundException,
                "[{ProjectionName}] Could not find {OrganisationId} in ES while processing envelope #{EnvelopeNumber}, adding it to organisations to rebuild",
                ProjectionName,
                organisationNotFoundException.OrganisationId,
                newLastProcessedEventNumber);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(0, ex, "[{ProjectionName}] An exception occurred while processing envelope #{EnvelopeNumber}", ProjectionName, newLastProcessedEventNumber);
            throw;
        }
    }

    private async Task ProcessChange(IElasticChange? changeSetChange, Dictionary<Guid, T> documentCache, int? eventNumber)
    {
        switch (changeSetChange)
        {
            case ElasticDocumentCreation<T> elasticDocumentCreation:
            {
                foreach (var documentChange in elasticDocumentCreation.Changes)
                {
                    var document = documentChange.Value();
                    documentCache.Add(documentChange.Key, document);
                }
                break;
            }
            case ElasticPerDocumentChange<T> perDocumentChange:
            {
                foreach (var documentChange in perDocumentChange.Changes)
                {
                    T? document;

                    if (!documentCache.ContainsKey(documentChange.Key))
                    {
                        document = (await _elastic.TryGetAsync(async () =>
                                (await _elastic.WriteClient.GetAsync<T>(documentChange.Key))
                                .ThrowOnFailure()))
                            .Source;

                        documentCache.Add(documentChange.Key, document);
                    }
                    else
                    {
                        document = documentCache[documentChange.Key];
                    }

                    await documentChange.Value(document);
                }

                break;
            }
            case ElasticMassChange massChange:
            {
                await FlushDocuments(documentCache);
                await massChange.Change(_elastic);
                await UpdateProjectionState(eventNumber);
                await _elastic.TryGetAsync(async () =>
                    (await _elastic.WriteClient.Indices.RefreshAsync(Indices.Index<T>())).ThrowOnFailure());
                break;
            }
        }
    }

    private async Task FlushDocuments(Dictionary<Guid, T> documentCache)
    {
        if (documentCache.Any())
        {
            if (documentCache.Any(x => x.Key == Guid.Empty || string.IsNullOrEmpty(x.Value.Name)))
            {
                throw new Exception("Found document without key or name.");
            }

            await _elastic.TryAsync(() =>
            {
                _elastic.WriteClient.BulkAll(documentCache.Values, b => b
                        .BackOffTime("30s")
                        .BackOffRetries(5)
                        .RefreshOnCompleted(false)
                        .MaxDegreeOfParallelism(Environment.ProcessorCount)
                        .Size(1000)
                    )
                    .Wait(TimeSpan.FromMinutes(15), next =>
                    {
                        _logger.LogInformation("[{ProjectionName}] Flushed documents, page {PageNumber}", ProjectionName, next.Page);
                    });

                return Task.CompletedTask;
            });
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

        _logger.LogInformation("[{ProjectionName}] Processed up until envelope #{LastProcessedEnvelopeNumber}", ProjectionName, newLastProcessedEventNumber);
        await _projectionStates.UpdateProjectionState(_elasticSearchProjectionsProjectionName, newLastProcessedEventNumber.Value);

        _metrics.LastProcessedEventNumber = new Measurement<int>(newLastProcessedEventNumber.Value);
    }

    private async Task<ElasticChanges> ProcessEnvelope(IEnvelope envelope)
    {
        var changes = await _bus.Publish(null, null, (dynamic) envelope);
        return new ElasticChanges(envelope.Number, changes);
    }
}
