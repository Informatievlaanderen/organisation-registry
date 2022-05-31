namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationBuildingAdded : BaseEvent<OrganisationBuildingAdded>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationBuildingId { get; }
    public Guid BuildingId { get; }
    public string BuildingName { get; }
    public bool IsMainBuilding { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public OrganisationBuildingAdded(
        Guid organisationId,
        Guid organisationBuildingId,
        Guid buildingId,
        string buildingName,
        bool isMainBuilding,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = organisationId;

        OrganisationBuildingId = organisationBuildingId;
        BuildingId = buildingId;
        BuildingName = buildingName;
        IsMainBuilding = isMainBuilding;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
