namespace OrganisationRegistry.OrganisationClassificationType.Commands;

public class UpdateOrganisationClassificationType : BaseCommand<OrganisationClassificationTypeId>
{
    public OrganisationClassificationTypeId OrganisationClassificationTypeId
        => Id;

    public OrganisationClassificationTypeName Name { get; }
    public bool? AllowDifferentClassificationsToOverlap { get; }

    public UpdateOrganisationClassificationType(
        OrganisationClassificationTypeId organisationClassificationTypeId,
        OrganisationClassificationTypeName name,
        bool? allowDifferentClassificationsToOverlap)
    {
        Id = organisationClassificationTypeId;
        Name = name;
        AllowDifferentClassificationsToOverlap = allowDifferentClassificationsToOverlap;
    }
}
