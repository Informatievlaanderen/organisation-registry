namespace OrganisationRegistry.Organisation
{
    public class LocationAlreadyCoupledToInThisPeriodException : DomainException
    {
        public LocationAlreadyCoupledToInThisPeriodException()
            : base("Deze locatie is in deze periode reeds gekoppeld aan de organisatie.") { }
    }
}
