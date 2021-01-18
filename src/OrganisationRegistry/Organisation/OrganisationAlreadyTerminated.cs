namespace OrganisationRegistry.Organisation
{
    public class OrganisationAlreadyTerminated: DomainException
    {
        public OrganisationAlreadyTerminated()
            : base("Deze organisatie is reeds stopgezet.") { }
    }
}
