namespace OrganisationRegistry.Body.Exceptions
{
    public class BodyAlreadyCoupledToOrganisationInThisPeriod : DomainException
    {
        public BodyAlreadyCoupledToOrganisationInThisPeriod()
            : base("Er is in deze periode reeds een organisatie gekoppeld aan het orgaan.") { }
    }
}

