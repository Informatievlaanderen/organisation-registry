namespace OrganisationRegistry.Organisation
{
    public class OrganisationAlreadyHasAMainBuildingInThisPeriodException : DomainException
    {
        public OrganisationAlreadyHasAMainBuildingInThisPeriodException()
            : base("Deze organisatie heeft reeds een hoofdgebouw binnen deze periode.") { }
    }
}
