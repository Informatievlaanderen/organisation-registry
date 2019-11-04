namespace OrganisationRegistry.Organisation
{
    using ContactType;

    public class Contact
    {
        public ContactType ContactType { get; }
        public string Value { get; }

        public Contact(
            ContactType contactType,
            string value)
        {
            ContactType = contactType;
            Value = value;
        }
    }
}
