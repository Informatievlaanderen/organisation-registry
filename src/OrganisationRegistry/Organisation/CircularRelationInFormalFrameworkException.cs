namespace OrganisationRegistry.Organisation
{
    public class CircularRelationInFormalFrameworkException : DomainException
    {
        public CircularRelationInFormalFrameworkException()
            : base("Deze actie zou leiden tot een circulaire relatie tussen organisaties binnen dit toepassingsgebied.") { }
    }
}
