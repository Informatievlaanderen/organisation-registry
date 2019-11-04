namespace OrganisationRegistry
{
    public class NameNotUniqueException : DomainException
    {
        public NameNotUniqueException()
            : base("Naam is niet uniek.") { }
    }
}
