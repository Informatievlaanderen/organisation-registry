namespace OrganisationRegistry.Organisation.Exceptions;

public class UserCannotCoupleKboLegalName : DomainException
{
    public UserCannotCoupleKboLegalName()
        : base("Dit benaming type kan niet handmatig gekoppeld worden aan de organisatie.") { }
}