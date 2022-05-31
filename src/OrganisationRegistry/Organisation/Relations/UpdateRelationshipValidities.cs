namespace OrganisationRegistry.Organisation;

using System;

public class UpdateRelationshipValidities : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public DateTime Date { get; }

    public UpdateRelationshipValidities(OrganisationId organisationId, DateTime date)
    {
        Id = organisationId;
        Date = date;
    }

    protected bool Equals(UpdateRelationshipValidities other)
        => OrganisationId.Equals(other.OrganisationId);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((UpdateRelationshipValidities) obj);
    }

    public override int GetHashCode()
        => OrganisationId.GetHashCode();
}
