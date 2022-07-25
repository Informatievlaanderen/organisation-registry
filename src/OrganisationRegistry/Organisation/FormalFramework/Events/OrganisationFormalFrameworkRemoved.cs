namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationFormalFrameworkRemoved : BaseEvent<OrganisationFormalFrameworkRemoved>
{
    public Guid OrganisationId
        => Id;

    public Guid OrganisationFormalFrameworkId { get; }

    public OrganisationFormalFrameworkRemoved(
        Guid organisationId,
        Guid organisationFormalFrameworkId)
    {
        Id = organisationId;

        OrganisationFormalFrameworkId = organisationFormalFrameworkId;
    }
}
