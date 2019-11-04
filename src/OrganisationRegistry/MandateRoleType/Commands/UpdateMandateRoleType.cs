namespace OrganisationRegistry.MandateRoleType.Commands
{
    public class UpdateMandateRoleType : BaseCommand<MandateRoleTypeId>
    {
        public MandateRoleTypeId MandateRoleTypeId => Id;

        public string Name { get; }

        public UpdateMandateRoleType(
            MandateRoleTypeId mandateRoleTypeId,
            string name)
        {
            Id = mandateRoleTypeId;
            Name = name;
        }
    }
}
