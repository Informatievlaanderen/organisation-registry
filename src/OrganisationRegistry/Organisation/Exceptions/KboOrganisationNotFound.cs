namespace OrganisationRegistry.Organisation.Exceptions;

public class KboOrganisationNotFound : DomainException
{
    public KboOrganisationNotFound(string[] errorMessages)
        : base($"Organisatie werd niet gevonden in de VKBO. Foutmeldingen:\n - {string.Join("\n -", errorMessages)}") { }
}
