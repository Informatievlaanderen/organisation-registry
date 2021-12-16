namespace OrganisationRegistry.Organisation.Commands
{
    public class PlaceUnderVlimpersManagement : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public PlaceUnderVlimpersManagement(
            OrganisationId organisationId)
        {
            Id = organisationId;
        }
    }
}
