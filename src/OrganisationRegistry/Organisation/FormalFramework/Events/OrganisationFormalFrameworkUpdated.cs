namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationFormalFrameworkUpdated : BaseEvent<OrganisationFormalFrameworkUpdated>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationFormalFrameworkId { get; }

    public Guid FormalFrameworkId { get; }
    public string FormalFrameworkName { get; }

    public Guid ParentOrganisationId { get; }
    public Guid PreviousParentOrganisationId { get; }

    public string ParentOrganisationName { get; }
    public string PreviousParentOrganisationName { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public OrganisationFormalFrameworkUpdated(
        Guid organisationId,
        Guid organisationFormalFrameworkId,
        Guid formalFrameworkId,
        string formalFrameworkName,
        Guid parentOrganisationId,
        string parentOrganisationName,
        DateTime? validFrom,
        DateTime? validTo,
        Guid previousParentOrganisationId,
        string previousParentOrganisationName,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo)
    {
        Id = organisationId;

        OrganisationFormalFrameworkId = organisationFormalFrameworkId;
        FormalFrameworkId = formalFrameworkId;
        ParentOrganisationId = parentOrganisationId;
        ParentOrganisationName = parentOrganisationName;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousParentOrganisationName = previousParentOrganisationName;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
        FormalFrameworkName = formalFrameworkName;
        PreviousParentOrganisationId = previousParentOrganisationId;
    }
}
