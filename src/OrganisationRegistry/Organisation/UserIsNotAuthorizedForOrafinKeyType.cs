namespace OrganisationRegistry.Organisation
{
    public class UserIsNotAuthorizedForOrafinKeyType : DomainException
    {
        public UserIsNotAuthorizedForOrafinKeyType(): base("U hebt onvoldoende rechten voor dit informatiesysteem.")
        {

        }
    }
}
