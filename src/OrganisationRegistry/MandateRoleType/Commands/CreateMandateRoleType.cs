namespace OrganisationRegistry.MandateRoleType.Commands
{
    public class CreateMandateRoleType : BaseCommand<MandateRoleTypeId>
    {
        public MandateRoleTypeId MandateRoleTypeId => Id;

        public string Name { get; }

        public CreateMandateRoleType(
            MandateRoleTypeId mandateRoleTypeId,
            string name)
        {
            Id = mandateRoleTypeId;
            Name = name;
        }
    }
}
