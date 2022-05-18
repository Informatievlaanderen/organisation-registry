namespace OrganisationRegistry.Organisation;

using System;
using FormalFramework;

public class AddOrganisationFormalFramework : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public Guid OrganisationFormalFrameworkId { get; }
    public FormalFrameworkId FormalFrameworkId { get; }
    public OrganisationId ParentOrganisationId { get; }
    public ValidFrom ValidFrom { get; }
    public ValidTo ValidTo { get; }

    public AddOrganisationFormalFramework(
        Guid organisationFormalFrameworkId,
        FormalFrameworkId formalFrameworkId,
        OrganisationId organisationId,
        OrganisationId parentOrganisationId,
        ValidFrom validFrom,
        ValidTo validTo)
    {
        Id = organisationId;

        OrganisationFormalFrameworkId = organisationFormalFrameworkId;
        FormalFrameworkId = formalFrameworkId;
        ParentOrganisationId = parentOrganisationId;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
