namespace OrganisationRegistry.MandateRoleType.Commands;

public class UpdateMandateRoleType : BaseCommand<MandateRoleTypeId>
{
    public MandateRoleTypeId MandateRoleTypeId => Id;

    public MandateRoleTypeName Name { get; }

    public UpdateMandateRoleType(
        MandateRoleTypeId mandateRoleTypeId,
        MandateRoleTypeName name)
    {
        Id = mandateRoleTypeId;
        Name = name;
    }
}