namespace OrganisationRegistry.KboMutations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using SqlServer;
    using SqlServer.KboSyncQueue;

    class KboMutationsPersister : IKboMutationsPersister
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
                    context.KboSyncQueue.Add(new KboSyncQueueItem()
                    {
                        Id = Guid.NewGuid(),
                        SourceFileName = fullName,
                        SourceOrganisationKboNumber = mutation.Ondernemingsnummer,
                        SourceOrganisationName = mutation.MaatschappelijkeNaam,
                        SourceOrganisationModifiedAt = mutation.DatumModificatie,
                        MutationReadAt = DateTime.UtcNow,
                    });
                }

                context.SaveChanges();
            }
        }
    }
}
