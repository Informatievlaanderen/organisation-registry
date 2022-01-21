namespace OrganisationRegistry.Body.Exceptions
{
    public class BodyClassificationTypeAlreadyCoupledToInThisPeriod : DomainException
    {
        public BodyClassificationTypeAlreadyCoupledToInThisPeriod()
            : base("Dit classificatie type is in deze periode reeds gekoppeld aan het orgaan.") { }
    }
}
