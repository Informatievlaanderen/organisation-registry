namespace OrganisationRegistry.OrganisationClassificationType.Commands;

public class CreateOrganisationClassificationType : BaseCommand<OrganisationClassificationTypeId>
{
    public OrganisationClassificationTypeId OrganisationClassificationTypeId => Id;

    public OrganisationClassificationTypeName Name { get; }
    public bool AllowDifferentClassificationsToOverlap { get; }

    public CreateOrganisationClassificationType(
        OrganisationClassificationTypeId organisationClassificationTypeId,
        OrganisationClassificationTypeName name,
        bool allowDifferentClassificationsToOverlap)
    {
        Id = organisationClassificationTypeId;

        Name = name;
        AllowDifferentClassificationsToOverlap = allowDifferentClassificationsToOverlap;
    }
}
