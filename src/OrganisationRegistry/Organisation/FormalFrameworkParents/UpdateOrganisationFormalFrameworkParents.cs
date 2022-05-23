namespace OrganisationRegistry.Organisation;

using FormalFramework;

public class UpdateOrganisationFormalFrameworkParents : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;
    public FormalFrameworkId FormalFrameworkId { get; }

    public UpdateOrganisationFormalFrameworkParents(
        OrganisationId organisationId,
        FormalFrameworkId formalFrameworkId)
    {
        Id = organisationId;
        FormalFrameworkId = formalFrameworkId;
    }

    protected bool Equals(UpdateMainLocation other)
        => OrganisationId.Equals(other.OrganisationId);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((UpdateMainLocation)obj);
    }

    public override int GetHashCode()
        => OrganisationId.GetHashCode();
}
