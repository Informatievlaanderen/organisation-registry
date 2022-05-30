namespace OrganisationRegistry.OrganisationClassificationType.Commands;

public class UpdateOrganisationClassificationType : BaseCommand<OrganisationClassificationTypeId>
{
    public OrganisationClassificationTypeId OrganisationClassificationTypeId => Id;

    public OrganisationClassificationTypeName Name { get; }

    public UpdateOrganisationClassificationType(
        OrganisationClassificationTypeId organisationClassificationTypeId,
        OrganisationClassificationTypeName name)
    {
        Id = organisationClassificationTypeId;

        Name = name;
    }
}