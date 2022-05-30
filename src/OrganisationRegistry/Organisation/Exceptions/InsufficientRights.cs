namespace OrganisationRegistry.Organisation.Exceptions;

public class InsufficientRights : DomainException
{
    public InsufficientRights()
        : base("U heeft onvoldoende rechten voor deze actie.") { }
}