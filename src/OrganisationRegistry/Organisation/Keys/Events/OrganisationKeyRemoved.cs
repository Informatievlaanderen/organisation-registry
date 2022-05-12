namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationKeyRemoved : BaseEvent<OrganisationKeyRemoved>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationKeyId { get; }

    public OrganisationKeyRemoved(Guid organisationId, Guid organisationKeyId)
    {
        Id = organisationId;

        OrganisationKeyId = organisationKeyId;
    }
}
