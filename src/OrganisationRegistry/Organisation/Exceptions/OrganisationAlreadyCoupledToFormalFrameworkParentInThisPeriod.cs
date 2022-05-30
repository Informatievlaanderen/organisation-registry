namespace OrganisationRegistry.Organisation.Exceptions;

public class OrganisationAlreadyCoupledToFormalFrameworkParentInThisPeriod : DomainException
{
    public OrganisationAlreadyCoupledToFormalFrameworkParentInThisPeriod()
        : base("Deze organisatie is in deze periode reeds gekoppeld aan een moeder entiteit voor dit toepassingsgebied.") { }
}