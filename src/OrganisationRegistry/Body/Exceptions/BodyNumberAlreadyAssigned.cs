namespace OrganisationRegistry.Body.Exceptions;

public class BodyNumberAlreadyAssigned : DomainException
{
    public BodyNumberAlreadyAssigned()
        : base("Dit orgaan heeft reeds een nummer.") { }
}