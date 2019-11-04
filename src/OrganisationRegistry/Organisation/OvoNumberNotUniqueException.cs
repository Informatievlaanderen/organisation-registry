namespace OrganisationRegistry.Organisation
{
    public class OvoNumberNotUniqueException : DomainException
    {
        public OvoNumberNotUniqueException()
            : base("OVO-nummer is niet uniek.") { }
    }
}
