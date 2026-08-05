namespace OrganisationRegistry.Organisation.Exceptions;

public class InvalidKBONumber : DomainException
{
    public InvalidKBONumber()
        : base($"Ongeldig KBO-nummer.") { }
}
