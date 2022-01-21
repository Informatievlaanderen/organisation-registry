namespace OrganisationRegistry.Body.Exceptions
{
    public class IncorrectActiveLifecyclePhaseTypeDefinedInConfiguration : DomainException
    {
        public IncorrectActiveLifecyclePhaseTypeDefinedInConfiguration()
            : base("Er is geen gekende actieve standaard levensloopfase. Contacteer de beheerder om dit te corrigeren.") { }
    }
}

