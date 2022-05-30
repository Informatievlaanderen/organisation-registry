namespace OrganisationRegistry.KboMutations;

using System.Collections.Generic;

public record MutationsFile(string FullName, string Name, IEnumerable<MutationsLine> KboMutations);