namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationPlacedUnderVlimpersManagement : BaseEvent<OrganisationPlacedUnderVlimpersManagement>
{
    public Guid OrganisationId => Id;

    public OrganisationPlacedUnderVlimpersManagement(Guid organisationId)
    {
        Id = organisationId;
    }
}
