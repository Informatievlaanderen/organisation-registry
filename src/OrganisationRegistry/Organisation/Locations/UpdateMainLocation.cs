namespace OrganisationRegistry.Organisation;

public class UpdateMainLocation : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public UpdateMainLocation(OrganisationId organisationId)
    {
        Id = organisationId;
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
