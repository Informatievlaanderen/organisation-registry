namespace OrganisationRegistry.Organisation.Exceptions;

public class OrganisationAlreadyCoupledToParentInThisPeriod : DomainException
{
    public OrganisationAlreadyCoupledToParentInThisPeriod()
        : base("Deze organisatie is in deze periode reeds gekoppeld aan een moeder entiteit.") { }
}