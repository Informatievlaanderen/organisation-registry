namespace OrganisationRegistry.Body.Exceptions;

public class BodyNumberNotUnique : DomainException
{
    public BodyNumberNotUnique()
        : base("Orgaan nummer is niet uniek.") { }
}