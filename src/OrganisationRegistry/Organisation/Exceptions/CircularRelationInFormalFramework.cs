namespace OrganisationRegistry.Organisation.Exceptions;

public class CircularRelationInFormalFramework : DomainException
{
    public CircularRelationInFormalFramework()
        : base("Deze actie zou leiden tot een circulaire relatie tussen organisaties binnen dit toepassingsgebied.") { }
}
