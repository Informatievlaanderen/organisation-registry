namespace OrganisationRegistry.Exceptions;

public class RebuildProjectionRequiresAName : DomainException
{
    public RebuildProjectionRequiresAName()
        : base("Voor het rebuilden van de projections is een naam vereist.") { }
}
