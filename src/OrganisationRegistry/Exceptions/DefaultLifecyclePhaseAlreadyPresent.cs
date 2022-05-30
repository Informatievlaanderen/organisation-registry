namespace OrganisationRegistry.Exceptions;

using LifecyclePhaseType;

public class DefaultLifecyclePhaseAlreadyPresent : DomainException
{
    public DefaultLifecyclePhaseAlreadyPresent(LifecyclePhaseTypeIsRepresentativeFor representsActivePhase)
        : base($"Standaard levensloopfase is reeds gedefinieerd voor {(representsActivePhase == LifecyclePhaseTypeIsRepresentativeFor.ActivePhase ? "actieve" : "inactieve")} levensloopfase.") { }
}