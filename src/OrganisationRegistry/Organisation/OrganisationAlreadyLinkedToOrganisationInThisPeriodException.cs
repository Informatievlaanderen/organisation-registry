namespace OrganisationRegistry.Organisation
{
    public class OrganisationAlreadyLinkedToOrganisationInThisPeriodException : DomainException
    {
        public OrganisationAlreadyLinkedToOrganisationInThisPeriodException()
            : base("Deze organisatie is in deze periode reeds gekoppeld aan een organisatie voor dit relatie type.") { }
    }
}
