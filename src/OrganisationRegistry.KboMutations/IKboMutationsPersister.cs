namespace OrganisationRegistry.KboMutations
{
    using System;
    using System.Collections.Generic;
    using Autofac.Features.OwnedInstances;
    using SqlServer.Infrastructure;
    using SqlServer.KboSyncQueue;

    public interface IKboMutationsPersister
    {
        void Persist(string fileName, IEnumerable<MutationsLine> mutations);
    }

    class KboMutationsPersister : IKboMutationsPersister
    {
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;

        public KboMutationsPersister(Func<Owned<OrganisationRegistryContext>> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void Persist(string fileName, IEnumerable<MutationsLine> mutations)
        {
            using (var context = _contextFactory().Value)
            {
                foreach (var mutation in mutations)
                {
                    context.KboSyncQueue.Add(new KboSyncQueueItem()
                    {
                        Id = Guid.NewGuid(),
                        SourceFileName = fileName,
                        SourceKboNumber = mutation.Ondernemingsnummer,
                        SourceName = mutation.MaatschappelijkeNaam,
                        SourceAddressModifiedAt = mutation.AdresDatumModificatie,
                        SourceModifiedAt = mutation.DatumModificatie,
                        MutationReadAt = DateTime.UtcNow,
                    });
                }

                context.SaveChanges();
            }
        }
    }
}
