namespace OrganisationRegistry
{
    public class RebuildProjectionRequiresANameException : DomainException
    {
        public RebuildProjectionRequiresANameException()
            : base("Voor het rebuilden van de projections is een naam vereist.") { }
    }
}
