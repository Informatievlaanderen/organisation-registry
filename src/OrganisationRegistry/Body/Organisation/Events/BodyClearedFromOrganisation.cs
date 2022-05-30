namespace OrganisationRegistry.Body.Events;

using System;

public class BodyClearedFromOrganisation : BaseEvent<BodyClearedFromOrganisation>
{
    public Guid BodyId => Id;

    public Guid OrganisationId { get; }

    public BodyClearedFromOrganisation(
        Guid bodyId,
        Guid organisationId)
    {
        Id = bodyId;

        OrganisationId = organisationId;
    }
}