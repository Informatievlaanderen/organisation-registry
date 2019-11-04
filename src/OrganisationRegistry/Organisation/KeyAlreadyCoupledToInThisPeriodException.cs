namespace OrganisationRegistry.Organisation
{
    public class KeyAlreadyCoupledToInThisPeriodException : DomainException
    {
        public KeyAlreadyCoupledToInThisPeriodException()
            : base("Deze sleutel is in deze periode reeds gekoppeld aan de organisatie.") { }
    }
}
