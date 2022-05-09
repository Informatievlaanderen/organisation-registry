namespace OrganisationRegistry.Organisation.Events;

using System;
using System.Collections.Generic;

public class OrganisationPurposesUpdated : BaseEvent<OrganisationPurposesUpdated>
{
    public Guid OrganisationId
        => Id;

    public List<Purpose> Purposes { get; }

    public OrganisationPurposesUpdated(
        Guid organisationId,
        List<Purpose> purposes)
    {
        Id = organisationId;

        Purposes = purposes;
    }
}
