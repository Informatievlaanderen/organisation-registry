namespace OrganisationRegistry.Organisation;

public class RemoveOrganisationCapacity : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public OrganisationCapacityId OrganisationCapacityId { get; }

    public RemoveOrganisationCapacity(
        OrganisationId organisationId,
        OrganisationCapacityId organisationCapacityId)
    {
        OrganisationCapacityId = organisationCapacityId;
        Id = organisationId;
    }
}
