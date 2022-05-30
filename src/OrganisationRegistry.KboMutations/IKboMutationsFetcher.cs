namespace OrganisationRegistry.KboMutations;

using System.Collections.Generic;

public interface IKboMutationsFetcher
{
    IEnumerable<MutationsFile> GetKboMutationFiles();
    void Archive(MutationsFile file);
}