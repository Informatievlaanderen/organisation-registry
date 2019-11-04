namespace OrganisationRegistry.Body
{
    public class BodyAlreadyCoupledToFormalFrameworkInThisPeriodException: DomainException
    {
        public BodyAlreadyCoupledToFormalFrameworkInThisPeriodException()
            : base("Er is in deze periode reeds een toepassingsgebied gekoppeld aan het orgaan.") { }
    }
}
