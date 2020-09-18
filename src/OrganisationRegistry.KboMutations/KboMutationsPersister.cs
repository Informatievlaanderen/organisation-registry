namespace OrganisationRegistry.KboMutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using SqlServer;
    using SqlServer.KboSyncQueue;

    public class KboMutationsPersister : IKboMutationsPersister
    {
        private readonly IContextFactory _contextFactory;
        private readonly ILogger<KboMutationsPersister> _logger;

        public KboMutationsPersister(
            IContextFactory contextFactory,
            ILogger<KboMutationsPersister> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public void Persist(string fullName, IEnumerable<MutationsLine> mutations)
        {
            var mutationsLines = mutations.ToList();

            _logger.LogInformation(
                "Persisting {FileName} with {NumberOfMutationLines} mutation lines",
                fullName,
                mutationsLines.Count);

            using (var context = _contextFactory.Create())
            {
                foreach (var mutation in mutationsLines)
                {
                    switch (mutation.StatusCode)
                    {
                        case KboStatusCodes.Terminated:
                            context.KboTerminationSyncQueue.Add(new KboTerminationSyncQueueItem
                            {
                                Id = Guid.NewGuid(),
                                SourceFileName = fullName,
                                SourceOrganisationKboNumber = mutation.Ondernemingsnummer,
                                SourceOrganisationName = mutation.MaatschappelijkeNaam,
                                SourceOrganisationModifiedAt = mutation.DatumModificatie,
                                SourceOrganisationTerminationCode = mutation.StopzettingsCode,
                                SourceOrganisationTerminationReason = mutation.StopzettingsReden,
                                SourceOrganisationTerminationDate = mutation.StopzettingsDatum!.Value,
                                MutationReadAt = DateTime.UtcNow,
                            });
                            break;
                        default:
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
                            break;
                    }
                }

                context.SaveChanges();
            }
        }
    }
}
