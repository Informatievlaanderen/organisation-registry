namespace OrganisationRegistry.Organisation;

using System;
using Location;
using LocationType;

public class UpdateOrganisationLocation : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public Guid OrganisationLocationId { get; }
    public LocationId LocationId { get; }
    public bool IsMainLocation { get; }
    public LocationTypeId? LocationTypeId { get; }
    public ValidFrom ValidFrom { get; }
    public ValidTo ValidTo { get; }
    public Source Source { get; }

    public UpdateOrganisationLocation(
        Guid organisationLocationId,
        OrganisationId organisationId,
        LocationId locationId,
        bool isMainLocation,
        LocationTypeId? locationTypeId,
        ValidFrom validFrom,
        ValidTo validTo,
        Source source)
    {
        Id = organisationId;

        OrganisationLocationId = organisationLocationId;
        LocationId = locationId;
        IsMainLocation = isMainLocation;
        LocationTypeId = locationTypeId;
        ValidFrom = validFrom;
        ValidTo = validTo;
        Source = source;
    }
}
