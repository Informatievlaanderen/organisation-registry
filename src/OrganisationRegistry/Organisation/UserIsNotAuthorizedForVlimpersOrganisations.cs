namespace OrganisationRegistry.Organisation
{
    public class UserIsNotAuthorizedForVlimpersOrganisations : DomainException
    {
        public UserIsNotAuthorizedForVlimpersOrganisations(): base("U heeft onvoldoende rechten om deze actie uit te voeren op een door Vlimpers beheerde organisatie.")
        {

        }
    }
}
