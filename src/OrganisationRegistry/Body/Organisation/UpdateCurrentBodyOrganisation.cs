namespace OrganisationRegistry.Body;

public class UpdateCurrentBodyOrganisation : BaseCommand<BodyId>
{
    public BodyId BodyId => Id;

    public UpdateCurrentBodyOrganisation(BodyId bodyId)
        => Id = bodyId;

    protected bool Equals(UpdateCurrentBodyOrganisation other)
        => BodyId.Equals(other.BodyId);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((UpdateCurrentBodyOrganisation) obj);
    }

    public override int GetHashCode()
        => BodyId.GetHashCode();
}
