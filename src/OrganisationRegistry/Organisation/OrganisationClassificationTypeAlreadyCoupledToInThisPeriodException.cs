namespace OrganisationRegistry.Organisation
{
    public class OrganisationClassificationTypeAlreadyCoupledToInThisPeriodException : DomainException
    {
        public OrganisationClassificationTypeAlreadyCoupledToInThisPeriodException()
            : base("Dit classificatie type is in deze periode reeds gekoppeld aan de organisatie.") { }
    }
}
