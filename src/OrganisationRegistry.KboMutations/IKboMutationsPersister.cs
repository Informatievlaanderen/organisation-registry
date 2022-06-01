namespace OrganisationRegistry.KboMutations;

using System.Collections.Generic;
using SqlServer.Infrastructure;

public interface IKboMutationsPersister
{
    void Persist(
        OrganisationRegistryContext organisationRegistryContext,
        string fullName,
        IEnumerable<MutationsLine> mutations);
}
