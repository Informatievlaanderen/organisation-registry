namespace OrganisationRegistry.Body.Events;

using System;

public class BodyAssignedToOrganisation : BaseEvent<BodyAssignedToOrganisation>
{
    public Guid BodyId => Id;
    public string BodyName { get; }

    public Guid BodyOrganisationId { get; }

    public Guid OrganisationId { get; }
    public string OrganisationName { get; }

    public BodyAssignedToOrganisation(
        Guid bodyId,
        string bodyName,
        Guid organisationId,
        string organisationName,
        Guid bodyOrganisationId)
    {
        Id = bodyId;

        BodyName = bodyName;
        OrganisationId = organisationId;
        OrganisationName = organisationName;
        BodyOrganisationId = bodyOrganisationId;
    }
}