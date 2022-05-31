namespace OrganisationRegistry.Organisation.Exceptions;

public class KeyAlreadyCoupledToInThisPeriod : DomainException
{
    public KeyAlreadyCoupledToInThisPeriod()
        : base("Deze sleutel is in deze periode reeds gekoppeld aan de organisatie.") { }
}
