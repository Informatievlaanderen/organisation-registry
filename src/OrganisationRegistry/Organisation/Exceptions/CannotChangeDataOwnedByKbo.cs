namespace OrganisationRegistry.Organisation.Exceptions;

public class CannotChangeDataOwnedByKbo: DomainException
{
    public CannotChangeDataOwnedByKbo()
        : base("Deze data kan niet gewijzigd worden.") { }
}