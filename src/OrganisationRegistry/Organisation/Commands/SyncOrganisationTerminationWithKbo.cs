namespace OrganisationRegistry.Organisation.Commands;

public class SyncOrganisationTerminationWithKbo : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;


    public SyncOrganisationTerminationWithKbo(
        OrganisationId organisationId)
    {
        Id = organisationId;
    }
}