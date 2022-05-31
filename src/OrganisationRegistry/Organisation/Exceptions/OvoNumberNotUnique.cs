namespace OrganisationRegistry.Organisation.Exceptions;

public class OvoNumberNotUnique : DomainException
{
    public OvoNumberNotUnique()
        : base("OVO-nummer is niet uniek.") { }
}
