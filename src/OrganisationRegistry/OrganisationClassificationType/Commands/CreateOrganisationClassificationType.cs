namespace OrganisationRegistry.OrganisationClassificationType.Commands
{
    public class CreateOrganisationClassificationType : BaseCommand<OrganisationClassificationTypeId>
    {
        public OrganisationClassificationTypeId OrganisationClassificationTypeId => Id;

        public string Name { get; }

        public CreateOrganisationClassificationType(
            OrganisationClassificationTypeId organisationClassificationTypeId,
            string name)
        {
            Id = organisationClassificationTypeId;

            Name = name;
        }
    }
}
