namespace OrganisationRegistry.Organisation.Exceptions
{
    public class OrganisationClassificationTypeAlreadyCoupledToInThisPeriod : DomainException
    {
        public OrganisationClassificationTypeAlreadyCoupledToInThisPeriod()
            : base("Dit classificatie type is in deze periode reeds gekoppeld aan de organisatie.") { }
    }
}
