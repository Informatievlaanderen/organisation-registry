namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationParentAdded : BaseEvent<OrganisationParentAdded>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationOrganisationParentId { get; }
    public Guid ParentOrganisationId { get; }
    public string ParentOrganisationName { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public OrganisationParentAdded(
        Guid organisationId,
        Guid organisationOrganisationParentId,
        Guid parentOrganisationId,
        string parentOrganisationName,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = organisationId;

        OrganisationOrganisationParentId = organisationOrganisationParentId;
        ParentOrganisationId = parentOrganisationId;
        ParentOrganisationName = parentOrganisationName;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
