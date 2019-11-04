namespace OrganisationRegistry.Body
{
    public class BodyAlreadyCoupledToOrganisationInThisPeriodException : DomainException
    {
        public BodyAlreadyCoupledToOrganisationInThisPeriodException()
            : base("Er is in deze periode reeds een organisatie gekoppeld aan het orgaan.") { }
    }
}

