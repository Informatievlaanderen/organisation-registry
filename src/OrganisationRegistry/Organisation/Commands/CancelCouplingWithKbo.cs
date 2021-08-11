namespace OrganisationRegistry.Organisation.Commands
{
    public class CancelCouplingWithKbo : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;


        public CancelCouplingWithKbo(
            OrganisationId organisationId)
        {
            Id = organisationId;
        }
    }
}
