namespace OrganisationRegistry.Organisation.Commands
{
    public class RemoveOrganisationKey : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public OrganisationKeyId OrganisationKeyId { get; }

        public RemoveOrganisationKey(
            OrganisationId organisationId,
            OrganisationKeyId organisationKeyId)
        {
            OrganisationKeyId = organisationKeyId;
            Id = organisationId;
        }
    }
}
