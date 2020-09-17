namespace OrganisationRegistry.Organisation
{
    public class KboOrganisationNotTerminatedException : DomainException
    {
        public KboOrganisationNotTerminatedException()
            : base("Deze organisatie heeft geen stopzetting in de KBO.") { }
    }
}
