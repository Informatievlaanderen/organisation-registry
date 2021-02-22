namespace OrganisationRegistry.Organisation
{
    public class OrganisationAlreadyTerminatedInKbo: DomainException
    {
        public OrganisationAlreadyTerminatedInKbo()
            : base("Deze organisatie is reeds stopgezet in de KBO.") { }
    }
}
