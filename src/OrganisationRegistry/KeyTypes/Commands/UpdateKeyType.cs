namespace OrganisationRegistry.KeyTypes.Commands
{
    public class UpdateKeyType : BaseCommand<KeyTypeId>
    {
        public KeyTypeId KeyTypeId => Id;

        public string Name { get; }

        public UpdateKeyType(
            KeyTypeId keyTypeId,
            string name)
        {
            Id = keyTypeId;
            Name = name;
        }
    }
}
