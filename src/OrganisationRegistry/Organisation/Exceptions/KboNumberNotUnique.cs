namespace OrganisationRegistry.Organisation.Exceptions
{
    public class KboNumberNotUnique : DomainException
    {
        public KboNumberNotUnique()
            : base("Kbo-nummer is niet uniek.") { }
    }
}
