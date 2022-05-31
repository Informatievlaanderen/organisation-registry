namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationBuildingUpdated : BaseEvent<OrganisationBuildingUpdated>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationBuildingId { get; }

    public Guid BuildingId { get; }
    public Guid PreviousBuildingId { get; }

    public string BuildingName { get; }
    public string PreviousBuildingName { get; }

    public bool IsMainBuilding { get; }
    public bool WasPreviouslyMainBuilding { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public OrganisationBuildingUpdated(
        Guid organisationId,
        Guid organisationBuildingId,
        Guid buildingId,
        string buildingName,
        bool isMainBuilding,
        DateTime? validFrom,
        DateTime? validTo,
        Guid previousBuildingId,
        string previousBuildingName,
        bool wasPreviouslyMainBuilding,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo)
    {
        Id = organisationId;

        OrganisationBuildingId = organisationBuildingId;
        BuildingId = buildingId;
        BuildingName = buildingName;
        IsMainBuilding = isMainBuilding;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousBuildingId = previousBuildingId;
        PreviousBuildingName = previousBuildingName;
        WasPreviouslyMainBuilding = wasPreviouslyMainBuilding;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}
