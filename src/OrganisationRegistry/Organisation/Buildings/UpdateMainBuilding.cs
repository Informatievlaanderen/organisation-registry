namespace OrganisationRegistry.Organisation;

public class UpdateMainBuilding : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public UpdateMainBuilding(OrganisationId organisationId)
    {
        Id = organisationId;
    }
}
