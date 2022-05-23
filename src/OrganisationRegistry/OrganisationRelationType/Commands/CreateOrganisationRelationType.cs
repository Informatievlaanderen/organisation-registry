namespace OrganisationRegistry.OrganisationRelationType.Commands
{
    public class CreateOrganisationRelationType : BaseCommand<OrganisationRelationTypeId>
    {
        public OrganisationRelationTypeId OrganisationRelationTypeId => Id;

        public string Name { get; }
        public string? InverseName { get; }

        public CreateOrganisationRelationType(
            OrganisationRelationTypeId organisationRelationTypeId,
            string name,
            string? inverseName)
        {
            Id = organisationRelationTypeId;

            Name = name;
            InverseName = inverseName;
        }
    }
}
