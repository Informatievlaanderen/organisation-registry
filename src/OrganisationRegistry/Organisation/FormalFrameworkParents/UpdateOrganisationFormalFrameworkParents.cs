namespace OrganisationRegistry.Organisation;

using System;
using FormalFramework;

public class UpdateOrganisationFormalFrameworkParents : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId
        => Id;

    public FormalFrameworkId FormalFrameworkId { get; }

    public UpdateOrganisationFormalFrameworkParents(
        OrganisationId organisationId,
        FormalFrameworkId formalFrameworkId)
    {
        Id = organisationId;
        FormalFrameworkId = formalFrameworkId;
    }

    protected bool Equals(UpdateOrganisationFormalFrameworkParents other)
        => OrganisationId.Equals(other.OrganisationId)
           && FormalFrameworkId.Equals(other.FormalFrameworkId);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((UpdateOrganisationFormalFrameworkParents)obj);
    }

    public override int GetHashCode()
        => HashCode.Combine(OrganisationId, FormalFrameworkId);
}
