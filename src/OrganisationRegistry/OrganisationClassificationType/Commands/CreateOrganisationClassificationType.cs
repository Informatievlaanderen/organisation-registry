namespace OrganisationRegistry.OrganisationClassificationType.Commands;

public class CreateOrganisationClassificationType : BaseCommand<OrganisationClassificationTypeId>
{
    public OrganisationClassificationTypeId OrganisationClassificationTypeId => Id;

    public OrganisationClassificationTypeName Name { get; }

    public CreateOrganisationClassificationType(
        OrganisationClassificationTypeId organisationClassificationTypeId,
        OrganisationClassificationTypeName name)
    {
        Id = organisationClassificationTypeId;

        Name = name;
    }
}