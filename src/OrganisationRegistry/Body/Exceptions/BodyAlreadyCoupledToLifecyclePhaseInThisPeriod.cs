namespace OrganisationRegistry.Body.Exceptions;

public class BodyAlreadyCoupledToLifecyclePhaseInThisPeriod : DomainException
{
    public BodyAlreadyCoupledToLifecyclePhaseInThisPeriod()
        : base("Er is in deze periode reeds een levensloopfase gekoppeld aan het orgaan.") { }
}
