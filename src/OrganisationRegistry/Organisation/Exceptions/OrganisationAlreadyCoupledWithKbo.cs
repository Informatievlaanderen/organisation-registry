namespace OrganisationRegistry.Organisation.Exceptions;

public class OrganisationAlreadyCoupledWithKbo: DomainException
{
    public OrganisationAlreadyCoupledWithKbo()
        : base("Deze organisatie is reeds gekoppeld aan een KBO nummer.") { }
}
