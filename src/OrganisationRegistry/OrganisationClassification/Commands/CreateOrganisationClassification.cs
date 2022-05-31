namespace OrganisationRegistry.OrganisationClassification.Commands;

using OrganisationClassificationType;

public class CreateOrganisationClassification : BaseCommand<OrganisationClassificationId>
{
    public OrganisationClassificationId OrganisationClassificationId => Id;

    public OrganisationClassificationName Name { get; }
    public int Order { get; }
    public string? ExternalKey { get; }
    public bool Active { get; }
    public OrganisationClassificationTypeId OrganisationClassificationTypeId { get; }

    public CreateOrganisationClassification(
        OrganisationClassificationId organisationClassificationId,
        OrganisationClassificationName name,
        int order,
        string? externalKey,
        bool active,
        OrganisationClassificationTypeId organisationClassificationTypeId)
    {
        Id = organisationClassificationId;

        Name = name;
        Order = order;
        ExternalKey = externalKey;
        Active = active;
        OrganisationClassificationTypeId = organisationClassificationTypeId;
    }
}
