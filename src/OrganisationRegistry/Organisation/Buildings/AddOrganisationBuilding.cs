namespace OrganisationRegistry.Organisation;

using System;
using Building;

public class AddOrganisationBuilding : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public Guid OrganisationBuildingId { get; }
    public BuildingId BuildingId { get; }
    public bool IsMainBuilding { get; }
    public ValidFrom ValidFrom { get; }
    public ValidTo ValidTo { get; }

    public AddOrganisationBuilding(
        Guid organisationBuildingId,
        OrganisationId organisationId,
        BuildingId buildingId,
        bool isMainBuilding,
        ValidFrom validFrom,
        ValidTo validTo)
    {
        Id = organisationId;

        OrganisationBuildingId = organisationBuildingId;
        BuildingId = buildingId;
        IsMainBuilding = isMainBuilding;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
