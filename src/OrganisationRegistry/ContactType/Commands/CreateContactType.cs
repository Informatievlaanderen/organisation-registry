namespace OrganisationRegistry.ContactType.Commands
{
    public class CreateContactType : BaseCommand<ContactTypeId>
    {
        public ContactTypeId ContactTypeId => Id;

        public string Name { get; }

        public CreateContactType(
            ContactTypeId contactTypeId,
            string name)
        {
            Id = contactTypeId;
            Name = name;
        }
    }
}
