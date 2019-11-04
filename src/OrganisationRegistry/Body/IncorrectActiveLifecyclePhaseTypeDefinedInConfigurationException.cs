namespace OrganisationRegistry.Body
{
    public class IncorrectActiveLifecyclePhaseTypeDefinedInConfigurationException : DomainException
    {
        public IncorrectActiveLifecyclePhaseTypeDefinedInConfigurationException()
            : base("Er is geen gekende actieve standaard levensloopfase. Contacteer de beheerder om dit te corrigeren.") { }
    }
}

