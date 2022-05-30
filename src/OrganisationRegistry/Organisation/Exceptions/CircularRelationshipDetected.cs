namespace OrganisationRegistry.Organisation.Exceptions;

public class CircularRelationshipDetected : DomainException
{
    public CircularRelationshipDetected()
        : base("Deze actie zou leiden tot een circulaire relatie tussen organisaties.") { }
}