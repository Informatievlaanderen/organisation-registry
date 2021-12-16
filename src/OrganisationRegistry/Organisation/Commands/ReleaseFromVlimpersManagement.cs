namespace OrganisationRegistry.Organisation.Commands
{
    public class ReleaseFromVlimpersManagement : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public ReleaseFromVlimpersManagement(
            OrganisationId organisationId)
        {
            Id = organisationId;
        }
    }
}
