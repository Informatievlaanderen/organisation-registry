namespace OrganisationRegistry.Organisation
{
    public class UserCannotCoupleKboLegalFormOrganisationClassification : DomainException
    {
        public UserCannotCoupleKboLegalFormOrganisationClassification()
            : base("Dit classificatie type kan niet handmatig gekoppeld worden aan de organisatie.") { }
    }
}
