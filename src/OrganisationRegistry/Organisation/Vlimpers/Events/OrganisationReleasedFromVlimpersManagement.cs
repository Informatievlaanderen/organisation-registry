namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationReleasedFromVlimpersManagement : BaseEvent<OrganisationReleasedFromVlimpersManagement>
{
    public Guid OrganisationId => Id;

    public OrganisationReleasedFromVlimpersManagement(Guid organisationId)
    {
        Id = organisationId;
    }
}