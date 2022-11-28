namespace OrganisationRegistry.Organisation.Exceptions;

public class LocationIsKboLocation : DomainException
{
    public LocationIsKboLocation() : base("Deze locatie is afkomstig uit het KBO.")
    {
    }
}
