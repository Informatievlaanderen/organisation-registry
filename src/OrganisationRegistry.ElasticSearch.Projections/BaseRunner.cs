namespace OrganisationRegistry.ElasticSearch.Projections;

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Body;
using Client;
using Configuration;
using ElasticSearch.Bodies;
using ElasticSearch.Organisations;
using ElasticSearch.People;
using Infrastructure;
using Infrastructure.Change;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using Organisation.Events;
using Osc;
using OrganisationRegistry.Infrastructure.Events;
using Organisations;
using SqlServer;
using SqlServer.ElasticSearchProjections;
using SqlServer.Event;
using SqlServer.Infrastructure;
using SqlServer.ProjectionState;

public record ProjectionName(string ElasticSearchProjectionsProjectionName, string FullName, string Name);
public abstract class BaseRunner<T> where T: class, IDocument, new()
{
    public string ProjectionName
        => _projectionName.Name;

    public Type[] EventHandlers { get; }

    private readonly Elastic _elastic;

    private readonly int _batchSize;
    private readonly ILogger<BaseRunner<T>> _logger;
    private readonly IEventStore _store;
    private readonly IProjectionStates _projectionStates;
    private readonly ElasticBus _bus;
    public IContextFactory ContextFactory { get; }
    private readonly OpenTelemetryMetrics.ElasticSearchProjections _metrics;
    private readonly ProjectionName _projectionName;

    protected BaseRunner(
        ILogger<BaseRunner<T>> logger,
        IOptions<ElasticSearchConfiguration> configuration,
        IEventStore store,
        IProjectionStates projectionStates,
        Type[] eventHandlers,
        Elastic elastic,
        ElasticBus bus,
        IContextFactory contextFactory,
        ProjectionName projectionName)
    {
        _logger = logger;
        _store = store;
        _projectionStates = projectionStates;
        _bus = bus;
        ContextFactory = contextFactory;
        _projectionName = projectionName;

        _batchSize = configuration.Value.BatchSize;
        _elastic = elastic;

        EventHandlers = eventHandlers;

        _metrics = new OpenTelemetryMetrics.ElasticSearchProjections(_projectionName.Name);
    }

    public async Task Run()
    {
        var maxEventNumberToProcess = _store.GetLastEvent();
        _metrics.MaxEventNumberToProcessGauge = _metrics.MaxEventNumberToProcessCounter = maxEventNumberToProcess;

        var lastProcessedEventNumber = await _projectionStates.GetLastProcessedEventNumber(_projectionName.ElasticSearchProjectionsProjectionName);
        var envelopesBehind = maxEventNumberToProcess - lastProcessedEventNumber;
        _metrics.NumberOfEnvelopesBehindGauge = _metrics.NumberOfEnvelopesBehindCounter = envelopesBehind;
        _metrics.NumberOfEnvelopesBehindHistogram.Record(envelopesBehind);

        await InitialiseProjection(lastProcessedEventNumber);

        var eventsBeingListenedTo =
            EventHandlers
                .SelectMany(
                    handler => handler
                        .GetTypeInfo()
                        .ImplementedInterfaces
                        .SelectMany(@interface => @interface.GenericTypeArguments))
                .Distinct()
                .ToList();

        var envelopes = _store
            .GetEventEnvelopesAfter(lastProcessedEventNumber, _batchSize, eventsBeingListenedTo.ToArray())
            .ToList();

        if (!envelopes.Any())
        {
            _metrics.NumberOfEnvelopesHandledHistogram.Record(0);
            _metrics.NumberOfEnvelopesHandledGauge = 0;
            _metrics.NumberOfEnvelopesHandledCounter = 0;
            return;
        }

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

                var changes = changeSet.Changes.ToList();
                for (var i = 0; i < changes.Count; i++)
                {
                    await ProcessChange(changes[i], documentCache, newLastProcessedEventNumber, i == changes.Count - 1);
                }
            }

            await FlushDocuments(documentCache);
            await UpdateProjectionState(newLastProcessedEventNumber);

            _logger.LogDebug("[{ProjectionName}] Succesfully handled {NumberOfEnvelopesHandled}", ProjectionName, envelopes.Count);
            _metrics.NumberOfEnvelopesHandledHistogram.Record(envelopes.Count);
            _metrics.NumberOfEnvelopesHandledGauge = envelopes.Count;
            _metrics.NumberOfEnvelopesHandledCounter = envelopes.Count;
        }
        catch (ElasticsearchAggregateNotFoundException aggregateNotFoundException)
        {
            // The document type that was missing decides which rebuild queue it goes in: a person
            // that 404s while we're projecting body events has to be rebuilt as a person, not as a body.
            if (!await QueueForRebuild(
                    aggregateNotFoundException.AggregateType,
                    Guid.Parse(aggregateNotFoundException.AggregateId)))
                throw;

            _logger.LogWarning(
                0,
                aggregateNotFoundException,
                "[{ProjectionName}] Could not find {DocumentType} {AggregateId} in ES while processing envelope #{EnvelopeNumber}, adding it to entities to rebuild",
                ProjectionName,
                aggregateNotFoundException.AggregateType.Name,
                aggregateNotFoundException.AggregateId,
                newLastProcessedEventNumber);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(0, ex, "[{ProjectionName}] An exception occurred while processing envelope #{EnvelopeNumber}", ProjectionName, newLastProcessedEventNumber);
            throw;
        }
    }

    private static void HandleOrganisation(OrganisationRegistryContext context, Guid aggregateId)
        => context.OrganisationsToRebuild.Add(new OrganisationToRebuild { OrganisationId = aggregateId });

    private static void HandleBody(OrganisationRegistryContext context, Guid aggregateId)
        => context.BodiesToRebuild.Add(new BodyToRebuild { BodyId = aggregateId });

    private static void HandlePerson(OrganisationRegistryContext context, Guid aggregateId)
        => context.PeopleToRebuild.Add(new PersonToRebuild { PersonId = aggregateId });

    /// <summary>
    /// Queues <paramref name="aggregateId"/> in the rebuild table belonging to <paramref name="documentType"/>.
    /// Returns false when the document type has no rebuild queue, so the caller can rethrow instead.
    /// </summary>
    private async Task<bool> QueueForRebuild(Type documentType, Guid aggregateId)
    {
        Action<OrganisationRegistryContext, Guid>? handle = documentType switch
        {
            { } t when t == typeof(OrganisationDocument) => HandleOrganisation,
            { } t when t == typeof(BodyDocument) => HandleBody,
            { } t when t == typeof(PersonDocument) => HandlePerson,
            _ => null,
        };

        if (handle is null)
            return false;

        await using var organisationRegistryContext = ContextFactory.Create();
        handle(organisationRegistryContext, aggregateId);
        await organisationRegistryContext.SaveChangesAsync();

        return true;
    }

    private async Task ProcessChange(IElasticChange? changeSetChange, Dictionary<Guid, T> documentCache, int? eventNumber, bool isLastChangeInSet)
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
                await HandlePerDocumentChange(documentCache, perDocumentChange);

                break;
            }
            case ElasticMassChange massChange:
            {
                await FlushDocuments(documentCache);
                await massChange.Change(_elastic);

                if(isLastChangeInSet) // don't update  projection state if this is not the last change!
                    await UpdateProjectionState(eventNumber);

                await _elastic.TryGetAsync(async () =>
                    (await _elastic.WriteClient.Indices.RefreshAsync(Indices.Index<T>())).ThrowOnFailure());
                break;
            }
        }
    }

    private async Task HandlePerDocumentChange(Dictionary<Guid, T> documentCache, ElasticPerDocumentChange<T> perDocumentChange)
    {
        try
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
        }
        catch (ElasticsearchPerDocumentChangeException e)
        {
            // A failed change always concerns this runner's own document type.
            await QueueForRebuild(typeof(T), e.AggregateId);

            _logger.LogWarning(
                0,
                e,
                "[{ProjectionName}] Error occured for {DocumentType} {AggregateId} in ES while processing envelope #{EnvelopeNumber}, adding it to entities to rebuild",
                ProjectionName,
                typeof(T).Name,
                e.AggregateId,
                e.EnvelopeNumber);

            throw;
        }
    }

    private async Task FlushDocuments(Dictionary<Guid, T> documentCache)
    {
        if (documentCache.Any())
        {
            documentCache.ThrowOnDocumentsWithoutKeyOrName(ProjectionName);

            await _elastic.TryAsync(
                () =>
                {
                    _elastic.WriteClient.BulkAll(
                            documentCache.Values,
                            b => b
                                .BackOffTime("30s")
                                .BackOffRetries(5)
                                .RefreshOnCompleted(false)
                                .MaxDegreeOfParallelism(Environment.ProcessorCount)
                                .Size(1000)
                        )
                        .Wait(TimeSpan.FromMinutes(15), next => { _logger.LogInformation("[{ProjectionName}] Flushed documents, page {PageNumber}", ProjectionName, next.Page); });

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
        await ProcessEnvelope(new InitialiseProjection(_projectionName.FullName).ToTypedEnvelope());
    }

    private async Task UpdateProjectionState(int? newLastProcessedEventNumber)
    {
        if (!newLastProcessedEventNumber.HasValue)
            return;

        _logger.LogInformation("[{ProjectionName}] Processed up until envelope #{LastProcessedEnvelopeNumber}", ProjectionName, newLastProcessedEventNumber);
        await _projectionStates.UpdateProjectionState(_projectionName.ElasticSearchProjectionsProjectionName, newLastProcessedEventNumber.Value);

        _metrics.LastProcessedEventNumberGauge = _metrics.LastProcessedEventNumberCounter = newLastProcessedEventNumber.Value;
    }

    private async Task<ElasticChanges> ProcessEnvelope(IEnvelope envelope)
    {
        var changes = await _bus.Publish(null, null, (dynamic)envelope);
        return new ElasticChanges(envelope.Number, changes);
    }
}
