namespace OrganisationRegistry.Body.Exceptions;

public class IncorrectInactiveLifecyclePhaseTypeDefinedInConfiguration : DomainException
{
    public IncorrectInactiveLifecyclePhaseTypeDefinedInConfiguration()
        : base("Er is geen gekende inactieve standaard levensloopfase. Contacteer de beheerder om dit te corrigeren.") { }
}