namespace OrganisationRegistry.Organisation
{
    public class KboOrganisationNotFoundException : DomainException
    {
        public KboOrganisationNotFoundException()
            : base("Organisatie werd niet gevonden in de VKBO.") { }
    }
}
