namespace OrganisationRegistry.KboMutations
{
    using System.Collections.Generic;

    public interface IKboMutationsPersister
    {
        void Persist(string fullName, IEnumerable<MutationsLine> mutations);
    }
}
