namespace OrganisationRegistry.KboMutations
{
    using System.Collections.Generic;
    using Autofac.Features.OwnedInstances;
    using SqlServer.Infrastructure;

    public interface IKboMutationsPersister
    {
        void Persist(string fullName, IEnumerable<MutationsLine> mutations);
    }
}
