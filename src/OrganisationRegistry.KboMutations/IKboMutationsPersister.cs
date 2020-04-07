namespace OrganisationRegistry.KboMutations
{
    using System;
    using System.Collections.Generic;
    using Autofac.Features.OwnedInstances;
    using SqlServer;
    using SqlServer.Infrastructure;
    using SqlServer.KboSyncQueue;

    public interface IKboMutationsPersister
    {
        void Persist(string fileName, IEnumerable<MutationsLine> mutations);
    }

    class KboMutationsPersister : IKboMutationsPersister
    {
        private readonly IContextFactory _contextFactory;
        public KboMutationsPersister(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void Persist(string fileName, IEnumerable<MutationsLine> mutations)
        {
            using (var context = _contextFactory.Create())
            {
                foreach (var mutation in mutations)
                {
                    context.KboSyncQueue.Add(new KboSyncQueueItem()
                    {
                        Id = Guid.NewGuid(),
                        SourceFileName = fileName,
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
