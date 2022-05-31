namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationLocationUpdated : BaseEvent<OrganisationLocationUpdated>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationLocationId { get; }

    public Guid LocationId { get; }
    public Guid PreviousLocationId { get; }

    public string LocationFormattedAddress { get; }
    public string PreviousLocationFormattedAddress { get; }

    public bool IsMainLocation { get; }
    public bool PreviousIsMainLocation { get; }

    public Guid? LocationTypeId { get; }
    public Guid? PreviousLocationTypeId { get; }

    public string? LocationTypeName { get; }
    public string? PreviousLocationTypeName { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public OrganisationLocationUpdated(
        Guid organisationId,
        Guid organisationLocationId,
        Guid locationId,
        string locationFormattedAddress,
        bool isMainLocation,
        Guid? locationTypeId,
        string? locationTypeName,
        DateTime? validFrom,
        DateTime? validTo,
        Guid previousLocationId,
        string previousLocationFormattedAddress,
        bool previousIsMainLocation,
        Guid? previousLocationTypeId,
        string? previousLocationTypeName,
        DateTime? previouslyValidFrom,
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

        PreviousLocationId = previousLocationId;
        PreviousLocationFormattedAddress = previousLocationFormattedAddress;
        PreviousIsMainLocation = previousIsMainLocation;
        PreviousLocationTypeId = previousLocationTypeId;
        PreviousLocationTypeName = previousLocationTypeName;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}
