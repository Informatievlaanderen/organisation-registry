namespace OrganisationRegistry.Organisation.Exceptions
{
    public class BuildingAlreadyCoupledToInThisPeriod : DomainException
    {
        public BuildingAlreadyCoupledToInThisPeriod()
            : base("Dit gebouw is in deze periode reeds gekoppeld aan de organisatie.") { }
    }
}
