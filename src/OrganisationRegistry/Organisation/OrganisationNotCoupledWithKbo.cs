namespace OrganisationRegistry.Organisation
{
    public class OrganisationNotCoupledWithKbo: DomainException
    {
        public OrganisationNotCoupledWithKbo()
            : base("Deze organisatie is niet gekoppeld aan een kbo nummer.") { }
    }
}
