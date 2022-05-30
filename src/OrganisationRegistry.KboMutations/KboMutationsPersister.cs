namespace OrganisationRegistry.KboMutations;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SqlServer.Infrastructure;
using SqlServer.KboSyncQueue;

public class KboMutationsPersister : IKboMutationsPersister
{
    private readonly ILogger<KboMutationsPersister> _logger;

    public KboMutationsPersister(ILogger<KboMutationsPersister> logger)
    {
        _logger = logger;
    }

    public void Persist(
        OrganisationRegistryContext context,
        string fullName,
        IEnumerable<MutationsLine> mutations)
    {
        var mutationsLines = mutations.ToList();

        _logger.LogInformation(
            "Persisting {FileName} with {NumberOfMutationLines} mutation lines",
            fullName,
            mutationsLines.Count);

        foreach (var mutation in mutationsLines)
        {
            context.KboSyncQueue.Add(new KboSyncQueueItem
            {
                Id = Guid.NewGuid(),
                SourceFileName = fullName,
                SourceOrganisationStatus = mutation.StatusCode,
                SourceOrganisationKboNumber = mutation.Ondernemingsnummer,
                SourceOrganisationName = mutation.MaatschappelijkeNaam,
                SourceOrganisationModifiedAt = mutation.DatumModificatie,
                MutationReadAt = DateTime.UtcNow,
            });
        }

        context.SaveChanges();
    }
}