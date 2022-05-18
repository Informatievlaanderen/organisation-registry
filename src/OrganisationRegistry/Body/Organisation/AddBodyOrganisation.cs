namespace OrganisationRegistry.Body;

using Organisation;

public class AddBodyOrganisation : BaseCommand<BodyId>
{
    public BodyId BodyId => Id;

    public BodyOrganisationId BodyOrganisationId { get; }
    public OrganisationId OrganisationId { get; }
    public Period Validity { get; }

    public AddBodyOrganisation(
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
