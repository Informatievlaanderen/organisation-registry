namespace OrganisationRegistry.Organisation
{
    public class KboNumberNotUniqueException : DomainException
    {
        public KboNumberNotUniqueException()
            : base("Kbo-nummer is niet uniek.") { }
    }
}
