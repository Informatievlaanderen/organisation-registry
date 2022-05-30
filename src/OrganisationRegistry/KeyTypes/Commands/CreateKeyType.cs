namespace OrganisationRegistry.KeyTypes.Commands;

public class CreateKeyType : BaseCommand<KeyTypeId>
{
    public KeyTypeId KeyTypeId => Id;

    public KeyTypeName Name { get; }

    public CreateKeyType(
        KeyTypeId keyTypeId,
        KeyTypeName name)
    {
        Id = keyTypeId;

        Name = name;
    }
}