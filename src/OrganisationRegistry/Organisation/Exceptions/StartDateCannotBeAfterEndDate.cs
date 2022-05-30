namespace OrganisationRegistry.Organisation.Exceptions;

public class StartDateCannotBeAfterEndDate : DomainException
{
    public StartDateCannotBeAfterEndDate()
        : base("De startdatum mag niet na de einddatum vallen.") { }
}