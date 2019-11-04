namespace OrganisationRegistry.Organisation
{
    public class LocationNotFoundException : DomainException
    {
        public LocationNotFoundException(string location)
            : base($"De volgende locatie werd niet gevonden: {location}") { }
    }
}
