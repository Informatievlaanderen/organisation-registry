namespace OrganisationRegistry.Organisation.Exceptions
{
    public class OrganisationAlreadyHasAMainBuildingInThisPeriod : DomainException
    {
        public OrganisationAlreadyHasAMainBuildingInThisPeriod()
            : base("Deze organisatie heeft reeds een hoofdgebouw binnen deze periode.") { }
    }
}
