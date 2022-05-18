namespace OrganisationRegistry.Organisation;

public class UpdateMainBuilding : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public UpdateMainBuilding(OrganisationId organisationId)
    {
        Id = organisationId;
    }

    protected bool Equals(UpdateMainBuilding other)
        => OrganisationId.Equals(other.OrganisationId);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((UpdateMainBuilding) obj);
    }

    public override int GetHashCode()
        => OrganisationId.GetHashCode();
}
