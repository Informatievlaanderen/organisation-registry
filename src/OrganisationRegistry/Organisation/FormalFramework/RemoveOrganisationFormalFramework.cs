namespace OrganisationRegistry.Organisation;

using System;

public class RemoveOrganisationFormalFramework : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public Guid OrganisationFormalFrameworkId { get; }

    public RemoveOrganisationFormalFramework(Guid organisationFormalFrameworkId, OrganisationId organisationId)
    {
        OrganisationFormalFrameworkId = organisationFormalFrameworkId;
        Id = organisationId;
    }
}
