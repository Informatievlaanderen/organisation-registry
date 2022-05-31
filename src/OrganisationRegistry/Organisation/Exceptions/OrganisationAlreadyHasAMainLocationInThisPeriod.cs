namespace OrganisationRegistry.Organisation.Exceptions;

public class OrganisationAlreadyHasAMainLocationInThisPeriod : DomainException
{
    public OrganisationAlreadyHasAMainLocationInThisPeriod()
        : base("Deze organisatie heeft reeds een hoofdlocatie binnen deze periode.") { }
}
