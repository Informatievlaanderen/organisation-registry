namespace OrganisationRegistry.Organisation.Events;

using System;

public class KboRegisteredOfficeOrganisationLocationRemoved : BaseEvent<KboRegisteredOfficeOrganisationLocationRemoved>
{
    public Guid OrganisationId => Id;
    public Guid OrganisationLocationId { get; }
    public Guid LocationId { get; }
    public string LocationFormattedAddress { get; }
    public bool IsMainLocation { get; }
    public Guid? LocationTypeId { get; set; }
    public string? LocationTypeName { get; set; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public KboRegisteredOfficeOrganisationLocationRemoved(
        Guid organisationId,
        Guid organisationLocationId,
        Guid locationId,
        string locationFormattedAddress,
        bool isMainLocation,
        Guid? locationTypeId,
        string? locationTypeName,
        DateTime? validFrom,
        DateTime? validTo,
        DateTime? previouslyValidTo)
    {
        Id = organisationId;
        OrganisationLocationId = organisationLocationId;
        LocationId = locationId;
        LocationFormattedAddress = locationFormattedAddress;
        IsMainLocation = isMainLocation;
        LocationTypeId = locationTypeId;
        LocationTypeName = locationTypeName;
        ValidFrom = validFrom;
        ValidTo = validTo;
        PreviouslyValidTo = previouslyValidTo;
    }
}
