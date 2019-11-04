namespace OrganisationRegistry.Body
{
    public class BodyNumberNotUniqueException : DomainException
    {
        public BodyNumberNotUniqueException()
            : base("Orgaan nummer is niet uniek.") { }
    }
}
