namespace OrganisationRegistry.Exceptions;

public class NameNotUnique : DomainException
{
    public NameNotUnique()
        : base("Naam is niet uniek.") { }
}
