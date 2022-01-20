namespace OrganisationRegistry.Organisation.Exceptions
{
    public class KboOrganisationTerminated : DomainException
    {
        public KboOrganisationTerminated()
            : base("Deze organisatie is stopgezet in de KBO. Enkel koppelingen aan een actief KBO nummer zijn toegelaten.") { }
    }
}
