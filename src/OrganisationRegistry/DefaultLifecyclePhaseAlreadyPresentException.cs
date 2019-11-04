namespace OrganisationRegistry
{
    using LifecyclePhaseType;

    public class DefaultLifecyclePhaseAlreadyPresentException : DomainException
    {
        public DefaultLifecyclePhaseAlreadyPresentException(LifecyclePhaseTypeIsRepresentativeFor representsActivePhase)
            : base($"Standaard levensloopfase is reeds gedefinieerd voor {(representsActivePhase == LifecyclePhaseTypeIsRepresentativeFor.ActivePhase ? "actieve" : "inactieve")} levensloopfase.") { }
    }
}
