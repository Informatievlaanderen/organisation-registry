namespace OrganisationRegistry.Organisation.Exceptions;

public class UserIsNotAuthorizedForOrganisation : DomainException
{
    public UserIsNotAuthorizedForOrganisation(): base("U heeft onvoldoende rechten om deze actie uit te voeren op deze organisatie.")
    {

    }
}
