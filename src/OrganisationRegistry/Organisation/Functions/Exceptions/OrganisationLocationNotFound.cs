namespace OrganisationRegistry.Organisation.Exceptions;

public class OrganisationLocationNotFound : DomainException
{
    public OrganisationLocationNotFound() : base("De opgegeven locatie werd niet gevonden bij de organisatie")
    {
    }
}
