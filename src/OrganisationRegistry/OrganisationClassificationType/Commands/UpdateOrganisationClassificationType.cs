namespace OrganisationRegistry.OrganisationClassificationType.Commands
{
    public class UpdateOrganisationClassificationType : BaseCommand<OrganisationClassificationTypeId>
    {
        public OrganisationClassificationTypeId OrganisationClassificationTypeId => Id;

        public string Name { get; }

        public UpdateOrganisationClassificationType(
            OrganisationClassificationTypeId organisationClassificationTypeId,
            string name)
        {
            Id = organisationClassificationTypeId;

            Name = name;
        }
    }
}
