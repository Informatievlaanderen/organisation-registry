namespace OrganisationRegistry.Organisation
{
    public class CircularRelationException : DomainException
    {
        public CircularRelationException()
            : base("Deze actie zou leiden tot een circulaire relatie tussen organisaties.") { }
    }
}
