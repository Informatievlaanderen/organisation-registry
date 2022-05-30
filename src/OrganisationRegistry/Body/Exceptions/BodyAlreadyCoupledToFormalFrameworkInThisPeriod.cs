namespace OrganisationRegistry.Body.Exceptions;

public class BodyAlreadyCoupledToFormalFrameworkInThisPeriod: DomainException
{
    public BodyAlreadyCoupledToFormalFrameworkInThisPeriod()
        : base("Er is in deze periode reeds een toepassingsgebied gekoppeld aan het orgaan.") { }
}