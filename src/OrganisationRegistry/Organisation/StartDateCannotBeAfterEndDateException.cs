namespace OrganisationRegistry.Organisation
{
    public class StartDateCannotBeAfterEndDateException : DomainException
    {
        public StartDateCannotBeAfterEndDateException()
            : base("De startdatum mag niet na de einddatum vallen.") { }
    }
}
