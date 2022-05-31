namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationBecameActive: BaseEvent<OrganisationBecameActive>
{
    public Guid OrganisationId => Id;

    public OrganisationBecameActive(Guid organisationId)
    {
        Id = organisationId;
    }
}
