namespace OrganisationRegistry.Organisation
{
    public class BuildingAlreadyCoupledToInThisPeriodException : DomainException
    {
        public BuildingAlreadyCoupledToInThisPeriodException()
            : base("Dit gebouw is in deze periode reeds gekoppeld aan de organisatie.") { }
    }
}
