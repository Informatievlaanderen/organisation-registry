namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Immutable;
using System.Linq;

public class CircularDependencyOrFaultyReferenceDiscoveredBetweenOrganisations : Exception
{
    public CircularDependencyOrFaultyReferenceDiscoveredBetweenOrganisations(ImmutableArray<(int linenumber, string reference)> faultyReferences)
        : base(ComposeMessage(faultyReferences))
    {
    }

    private static string ComposeMessage(ImmutableArray<(int linenumber, string reference)> faultyReferences)
        => Enumerable.Aggregate(faultyReferences, "Circulaire afhankelijkheden tussen organisaties of foutieve moeder-referentie(s) ontdekt:\n",
            (current, reference) => current + $"'{reference.reference}' op lijn {reference.linenumber}\n");
}
