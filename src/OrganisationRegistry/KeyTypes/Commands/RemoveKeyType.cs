namespace OrganisationRegistry.KeyTypes.Commands;

public class RemoveKeyType : BaseCommand<KeyTypeId>
{
    public KeyTypeId KeyTypeId
        => Id;

    public RemoveKeyType(KeyTypeId keyTypeId)
    {
        Id = keyTypeId;
    }
}
