namespace OrganisationRegistry
{
    public class CodeNotUniqueException : DomainException
    {
        public CodeNotUniqueException()
            : base("Code is niet uniek.") { }
    }
}
