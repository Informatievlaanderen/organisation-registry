namespace OrganisationRegistry.MandateRoleType.Commands;

public class CreateMandateRoleType : BaseCommand<MandateRoleTypeId>
{
    public MandateRoleTypeId MandateRoleTypeId => Id;

    public MandateRoleTypeName Name { get; }

    public CreateMandateRoleType(
        MandateRoleTypeId mandateRoleTypeId,
        MandateRoleTypeName name)
    {
        Id = mandateRoleTypeId;
        Name = name;
    }
}
