namespace OrganisationRegistry.Organisation;

public class UpdateMainLocation : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public UpdateMainLocation(OrganisationId organisationId)
    {
        Id = organisationId;
    }
}
