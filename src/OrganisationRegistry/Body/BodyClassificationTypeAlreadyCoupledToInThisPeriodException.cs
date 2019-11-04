namespace OrganisationRegistry.Body
{
    public class BodyClassificationTypeAlreadyCoupledToInThisPeriodException : DomainException
    {
        public BodyClassificationTypeAlreadyCoupledToInThisPeriodException()
            : base("Dit classificatie type is in deze periode reeds gekoppeld aan het orgaan.") { }
    }
}
