namespace OrganisationRegistry.ElasticSearch.Projections.People;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearch.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Client;
using Infrastructure;
using Infrastructure.Change;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer;
using SqlServer.ProjectionState;
using Osc;

public class IndividualPersonRebuildRunner
{
    public string ProjectionName => "IndividualRebuild";

    private readonly ILogger<IndividualPersonRebuildRunner> _logger;
    private readonly IEventStore _store;
    private readonly IContextFactory _contextFactory;
    private readonly IProjectionStates _projectionStates;
    private readonly ElasticBus _bus;
    private readonly Elastic _elastic;

    public IndividualPersonRebuildRunner(
        ILogger<IndividualPersonRebuildRunner> logger,
        IEventStore store,
        IContextFactory contextFactory,
        IProjectionStates projectionStates,
        ElasticBus bus,
        Elastic elastic,
        ElasticBusRegistrar busRegistrar)
    {
        _logger = logger;
        _store = store;
        _contextFactory = contextFactory;
        _projectionStates = projectionStates;
        _bus = bus;
        _elastic = elastic;

        busRegistrar.RegisterEventHandlers(PeopleRunner.EventHandlers);
    }

    public async Task Run()
    {
        await using var context = _contextFactory.Create();

        var lastProcessedEventNumber =
            await _projectionStates.GetLastProcessedEventNumber(PeopleRunner.ElasticSearchProjectionsProjectionName);

        var peopleToRebuild = await context.PeopleToRebuild.ToListAsync();

        if (peopleToRebuild.Count > 0)
            _logger.LogInformation("[{ProjectionName}] Found {NumberOfPeople} persons to rebuild", ProjectionName, peopleToRebuild.Count);

        try
        {
            foreach (var person in peopleToRebuild)
            {
                var envelopes = _store
                    .GetEventEnvelopesUntil<OrganisationRegistry.Person.Person>(
                        person.PersonId,
                        lastProcessedEventNumber)
                    .ToList();

                _logger.LogDebug("[{ProjectionName}] Found {NumberOfEnvelopes} envelopes (until #{MaxEventNumber}) to process for Person {PersonId}",
                    ProjectionName, envelopes.Count, envelopes.Last().Number, person.PersonId);

                var allChanges = new List<ElasticChanges>();
                foreach (var envelope in envelopes)
                {
                    var changes = await ProcessEnvelope(envelope);
                    allChanges.Add(changes);
                }

                var documentCache = new Dictionary<Guid, PersonDocument>();

                foreach (var changeSet in allChanges)
                {
                    foreach (var changeSetChange in changeSet.Changes)
                    {
                        await ProcessChange(changeSetChange, documentCache);
                    }
                }
                await FlushDocuments(documentCache);

                context.PeopleToRebuild.Remove(person);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(0, ex, "[{ProjectionName}] An exception occurred while handling envelopes", ProjectionName);
            throw;
        }
    }

    private async Task ProcessChange(IElasticChange? changeSetChange, Dictionary<Guid, PersonDocument> documentCache)
    {
        switch (changeSetChange)
        {
            case ElasticDocumentCreation<PersonDocument> elasticDocumentCreation:
            {
                foreach (var documentChange in elasticDocumentCreation.Changes)
                {
                    var document = documentChange.Value();
                    documentCache.Add(documentChange.Key, document);
                }
                break;
            }
            case ElasticPerDocumentChange<PersonDocument> perDocumentChange:
            {
                foreach (var documentChange in perDocumentChange.Changes)
                {
                    PersonDocument? document;

                    if (!documentCache.ContainsKey(documentChange.Key))
                    {
                        document = (await _elastic.TryGetAsync(async () =>
                                (await _elastic.WriteClient.GetAsync<PersonDocument>(documentChange.Key))
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
                await _elastic.TryGetAsync(async () =>
                    (await _elastic.WriteClient.Indices.RefreshAsync(Indices.Index<PersonDocument>())).ThrowOnFailure());
                break;
            }
        }
    }


    private async Task FlushDocuments(Dictionary<Guid, PersonDocument> documentCache)
    {
        if (documentCache.Any())
        {
            if (documentCache.Any(x => x.Key == Guid.Empty || string.IsNullOrEmpty(x.Value.Name)))
            {
                throw new Exception("Found document without key or name.");
            }

            await _elastic.TryAsync(async () =>
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
                        _logger.LogInformation("Wrote page {PageNumber}", next.Page);
                    });

                await Task.CompletedTask;
            });
            documentCache.Clear();
        }
    }


    private async Task<ElasticChanges> ProcessEnvelope(IEnvelope envelope)
    {
        var changes = await _bus.Publish(null, null, (dynamic) envelope);
        return new ElasticChanges(envelope.Number, changes);
    }
}
