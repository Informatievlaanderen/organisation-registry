namespace OrganisationRegistry.Organisation.Events;

using System;

public class ParentAssignedToOrganisation : BaseEvent<ParentAssignedToOrganisation>
{
    public Guid OrganisationId => Id;

    public Guid ParentOrganisationId { get; }
    public Guid OrganisationOrganisationParentId { get; }

    public ParentAssignedToOrganisation(
        Guid organisationId,
        Guid parentOrganisationId,
        Guid organisationOrganisationParentId)
    {
        Id = organisationId;

        ParentOrganisationId = parentOrganisationId;
        OrganisationOrganisationParentId = organisationOrganisationParentId;
    }
}