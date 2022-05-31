namespace OrganisationRegistry.Organisation.Exceptions;

public class LocationNotFound : DomainException
{
    public LocationNotFound(string location)
        : base($"De volgende locatie werd niet gevonden: {location}") { }
}
