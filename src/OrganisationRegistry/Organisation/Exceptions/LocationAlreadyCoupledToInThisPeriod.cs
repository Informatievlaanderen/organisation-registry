namespace OrganisationRegistry.Organisation.Exceptions;

public class LocationAlreadyCoupledToInThisPeriod : DomainException
{
    public LocationAlreadyCoupledToInThisPeriod()
        : base("Deze locatie is in deze periode reeds gekoppeld aan de organisatie.") { }
}