namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationBecameInactive: BaseEvent<OrganisationBecameInactive>
{
    public Guid OrganisationId => Id;

    public OrganisationBecameInactive(Guid organisationId)
    {
        Id = organisationId;
    }
}