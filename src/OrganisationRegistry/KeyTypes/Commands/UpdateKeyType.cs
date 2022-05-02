namespace OrganisationRegistry.KeyTypes.Commands
{
    public class UpdateKeyType : BaseCommand<KeyTypeId>
    {
        public KeyTypeId KeyTypeId
            => Id;

        public KeyTypeName Name { get; }

        public UpdateKeyType(
            KeyTypeId keyTypeId,
            KeyTypeName name)
        {
            Id = keyTypeId;

            Name = name;
        }
    }
}
