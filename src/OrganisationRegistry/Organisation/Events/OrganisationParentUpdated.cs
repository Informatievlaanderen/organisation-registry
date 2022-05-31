namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationParentUpdated : BaseEvent<OrganisationParentUpdated>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationOrganisationParentId { get; }

    public Guid ParentOrganisationId { get; }
    public Guid? PreviousParentOrganisationId { get; }

    public string ParentOrganisationName { get; }
    public string PreviousParentOrganisationName { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviousValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviousValidTo { get; }

    public OrganisationParentUpdated(
        Guid organisationId,
        Guid organisationOrganisationParentId,
        Guid parentOrganisationId,
        string parentOrganisationName, DateTime? validFrom,
        DateTime? validTo,
        Guid? previousParentOrganisationId,
        string previousParentOrganisationName,
        DateTime? previousValidFrom,
        DateTime? previousValidTo)
    {
        Id = organisationId;

        OrganisationOrganisationParentId = organisationOrganisationParentId;
        ParentOrganisationId = parentOrganisationId;
        ParentOrganisationName = parentOrganisationName;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousParentOrganisationId = previousParentOrganisationId;
        PreviousParentOrganisationName = previousParentOrganisationName;
        PreviousValidFrom = previousValidFrom;
        PreviousValidTo = previousValidTo;
    }
}
