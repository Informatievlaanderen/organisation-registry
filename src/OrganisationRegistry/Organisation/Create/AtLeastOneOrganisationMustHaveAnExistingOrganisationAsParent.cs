namespace OrganisationRegistry.Organisation;

using System;

public class AtLeastOneOrganisationMustHaveAnExistingOrganisationAsParent : Exception
{
    public AtLeastOneOrganisationMustHaveAnExistingOrganisationAsParent()
        : base("Minstens 1 organisatie moet een bestaande organisatie hebben als moeder")
    {
    }
}
