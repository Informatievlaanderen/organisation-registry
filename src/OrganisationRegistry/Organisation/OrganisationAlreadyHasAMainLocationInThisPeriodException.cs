namespace OrganisationRegistry.Organisation
{
    public class OrganisationAlreadyHasAMainLocationInThisPeriodException : DomainException
    {
        public OrganisationAlreadyHasAMainLocationInThisPeriodException()
            : base("Deze organisatie heeft reeds een hoofdlocatie binnen deze periode.") { }
    }
}
