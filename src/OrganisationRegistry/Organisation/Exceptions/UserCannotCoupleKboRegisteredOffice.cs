namespace OrganisationRegistry.Organisation.Exceptions;

public class UserCannotCoupleKboRegisteredOffice : DomainException
{
    public UserCannotCoupleKboRegisteredOffice()
        : base("Dit locatie type kan niet handmatig gekoppeld worden aan de organisatie.") { }
}