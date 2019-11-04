namespace OrganisationRegistry.Body
{
    public class IncorrectInactiveLifecyclePhaseTypeDefinedInConfigurationException : DomainException
    {
        public IncorrectInactiveLifecyclePhaseTypeDefinedInConfigurationException()
            : base("Er is geen gekende inactieve standaard levensloopfase. Contacteer de beheerder om dit te corrigeren.") { }
    }
}

