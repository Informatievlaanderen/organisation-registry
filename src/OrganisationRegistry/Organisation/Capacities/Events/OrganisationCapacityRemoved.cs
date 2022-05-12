namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationCapacityRemoved : BaseEvent<OrganisationCapacityRemoved>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationCapacityId { get; }

    public OrganisationCapacityRemoved(Guid organisationId, Guid organisationCapacityId)
    {
        Id = organisationId;

        OrganisationCapacityId = organisationCapacityId;
    }
}
