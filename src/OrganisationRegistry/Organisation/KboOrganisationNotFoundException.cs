namespace OrganisationRegistry.Organisation
{
    using System.Linq;

    public class KboOrganisationNotFoundException : DomainException
    {
        public KboOrganisationNotFoundException(string[] errorMessages)
            : base($"Organisatie werd niet gevonden in de VKBO. Foutmeldingen:\n - {string.Join("\n -", errorMessages)}") { }
    }
}
