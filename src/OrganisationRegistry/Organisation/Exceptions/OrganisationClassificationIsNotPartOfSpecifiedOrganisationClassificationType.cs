﻿namespace OrganisationRegistry.Organisation.Exceptions;

public class OrganisationClassificationIsNotPartOfSpecifiedOrganisationClassificationType:DomainException
{
    public OrganisationClassificationIsNotPartOfSpecifiedOrganisationClassificationType()
        : base("De opgegeven organisatie classificatie behoort niet tot het opgegeven organisatie classificatie type")
    {
    }
}
