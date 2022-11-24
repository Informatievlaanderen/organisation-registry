namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationLocationRemoved : BaseEvent<OrganisationLocationRemoved>
{
    public Guid OrganisationId
        => Id;

    public Guid OrganisationLocationId { get; }

    public OrganisationLocationRemoved(
        Guid organisationId,
        Guid organisationLocationId)
    {
        Id = organisationId;

        OrganisationLocationId = organisationLocationId;
    }
}
