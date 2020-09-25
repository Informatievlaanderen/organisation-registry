namespace OrganisationRegistry.Organisation
{
    public class KboOrganisationTerminatedException : DomainException
    {
        public KboOrganisationTerminatedException()
            : base("Deze organisatie is stopgezet in de KBO. Enkel koppelingen aan een actief KBO nummer zijn toegelaten.") { }
    }
}
