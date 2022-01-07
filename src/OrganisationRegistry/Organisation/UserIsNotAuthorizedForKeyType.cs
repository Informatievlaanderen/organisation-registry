namespace OrganisationRegistry.Organisation
{
    public class UserIsNotAuthorizedForKeyType : DomainException
    {
        public UserIsNotAuthorizedForKeyType(): base("U hebt onvoldoende rechten voor dit informatiesysteem.")
        {

        }
    }
}
