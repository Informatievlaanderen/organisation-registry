namespace OrganisationRegistry.Organisation
{
    public class LabelAlreadyCoupledToInThisPeriodException : DomainException
    {
        public LabelAlreadyCoupledToInThisPeriodException()
            : base("Dit label is in deze periode reeds gekoppeld aan de organisatie.") { }
    }
}
