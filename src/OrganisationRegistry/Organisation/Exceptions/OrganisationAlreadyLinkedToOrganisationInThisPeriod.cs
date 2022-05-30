namespace OrganisationRegistry.Organisation.Exceptions;

public class OrganisationAlreadyLinkedToOrganisationInThisPeriod : DomainException
{
    public OrganisationAlreadyLinkedToOrganisationInThisPeriod()
        : base("Deze organisatie is in deze periode reeds gekoppeld aan een organisatie voor dit relatie type.") { }
}