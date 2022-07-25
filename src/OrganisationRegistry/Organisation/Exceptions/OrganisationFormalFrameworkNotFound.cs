namespace OrganisationRegistry.Organisation.Exceptions;

public class OrganisationFormalFrameworkNotFound : DomainException
{
    public OrganisationFormalFrameworkNotFound() : base("Het opgegeven toepassingsgebied werd niet gevonden bij de organisatie")
    {
    }
}
