namespace OrganisationRegistry.Organisation
{
    public class OrganisationAlreadyCoupledToParentInThisPeriodException : DomainException
    {
        public OrganisationAlreadyCoupledToParentInThisPeriodException()
            : base("Deze organisatie is in deze periode reeds gekoppeld aan een moeder entiteit.") { }
    }
}
