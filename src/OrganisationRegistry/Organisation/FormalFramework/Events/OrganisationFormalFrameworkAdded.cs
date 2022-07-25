namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationFormalFrameworkAdded : BaseEvent<OrganisationFormalFrameworkAdded>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationFormalFrameworkId { get; }
    public Guid FormalFrameworkId { get; }
    public string FormalFrameworkName { get; }
    public Guid ParentOrganisationId { get; }
    public string ParentOrganisationName { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public OrganisationFormalFrameworkAdded(
        Guid organisationId,
        Guid organisationFormalFrameworkId,
        Guid formalFrameworkId,
        string formalFrameworkName,
        Guid parentOrganisationId,
        string parentOrganisationName,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = organisationId;

        OrganisationFormalFrameworkId = organisationFormalFrameworkId;
        FormalFrameworkId = formalFrameworkId;
        FormalFrameworkName = formalFrameworkName;
        ParentOrganisationId = parentOrganisationId;
        ParentOrganisationName = parentOrganisationName;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
