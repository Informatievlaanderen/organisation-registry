namespace OrganisationRegistry.Body
{
    public class BodyAlreadyCoupledToLifecyclePhaseInThisPeriodException : DomainException
    {
        public BodyAlreadyCoupledToLifecyclePhaseInThisPeriodException()
            : base("Er is in deze periode reeds een levensloopfase gekoppeld aan het orgaan.") { }
    }
}

