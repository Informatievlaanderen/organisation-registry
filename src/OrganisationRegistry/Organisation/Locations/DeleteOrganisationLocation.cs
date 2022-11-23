namespace OrganisationRegistry.Organisation;

using System;

public class DeleteOrganisationLocation : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;
    public Guid OrganisationLocationId { get; }

    public DeleteOrganisationLocation(OrganisationId organisationId, Guid organisationLocationId)
    {
        OrganisationLocationId = organisationLocationId;
        Id = organisationId;
    }
}
