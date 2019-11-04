namespace OrganisationRegistry.KeyTypes.Commands
{
    public class CreateKeyType : BaseCommand<KeyTypeId>
    {
        public KeyTypeId KeyTypeId => Id;

        public string Name { get; }

        public CreateKeyType(
            KeyTypeId keyTypeId,
            string name)
        {
            Id = keyTypeId;
            Name = name;
        }
    }
}
