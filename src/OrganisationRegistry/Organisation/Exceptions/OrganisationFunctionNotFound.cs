namespace OrganisationRegistry.Organisation.Exceptions;

public class OrganisationFunctionNotFound : DomainException
{
    public OrganisationFunctionNotFound() : base("De opgegeven functie werd niet gevonden bij de organisatie")
    {
    }
}
