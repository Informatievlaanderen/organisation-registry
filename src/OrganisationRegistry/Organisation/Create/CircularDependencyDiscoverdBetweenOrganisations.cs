namespace OrganisationRegistry.Organisation;

using System;

public class CircularDependencyDiscoverdBetweenOrganisations : Exception
{
    public CircularDependencyDiscoverdBetweenOrganisations()
        : base("Circulaire afhankelijkheden ontdekt tussen organisaties")
    {
    }
}
