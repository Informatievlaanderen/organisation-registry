namespace OrganisationRegistry.Organisation
{
    public class OrganisationAlreadyCoupledToFormalFrameworkParentInThisPeriodException : DomainException
    {
        public OrganisationAlreadyCoupledToFormalFrameworkParentInThisPeriodException()
            : base("Deze organisatie is in deze periode reeds gekoppeld aan een moeder entiteit voor dit toepassingsgebied.") { }
    }
}
