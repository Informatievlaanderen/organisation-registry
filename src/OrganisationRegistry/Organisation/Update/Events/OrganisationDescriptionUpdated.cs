namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationDescriptionUpdated : BaseEvent<OrganisationDescriptionUpdated>
{
    public Guid OrganisationId
        => Id;

    public string? Description { get; }

    public OrganisationDescriptionUpdated(
        Guid organisationId,
        string? description)
    {
        Id = organisationId;

        Description = description;
    }
}
