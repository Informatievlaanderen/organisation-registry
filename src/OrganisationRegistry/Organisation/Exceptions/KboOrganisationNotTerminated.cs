namespace OrganisationRegistry.Organisation.Exceptions
{
    public class KboOrganisationNotTerminated : DomainException
    {
        public KboOrganisationNotTerminated()
            : base("Deze organisatie heeft geen stopzetting in de KBO.") { }
    }
}
