namespace OrganisationRegistry.Exceptions
{
    public class CodeNotUnique : DomainException
    {
        public CodeNotUnique()
            : base("Code is niet uniek.") { }
    }
}
