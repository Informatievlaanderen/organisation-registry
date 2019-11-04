namespace OrganisationRegistry.Body.Commands
{
    using Organisation;

    public class UpdateBodyOrganisation : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public BodyOrganisationId BodyOrganisationId { get; }
        public OrganisationId OrganisationId { get; }
        public Period Validity { get; }

        public UpdateBodyOrganisation(
            BodyId bodyId,
            BodyOrganisationId bodyOrganisationId,
            OrganisationId organisationId,
            Period validity)
        {
            Id = bodyId;

            BodyOrganisationId = bodyOrganisationId;
            OrganisationId = organisationId;
            Validity = validity;
        }
    }
}
