namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using System.Linq;

public class OrganisationBuildings : List<OrganisationBuilding>
{
    public OrganisationBuildings() { }

    public OrganisationBuildings(IEnumerable<OrganisationBuilding> organisationBuildings) : base(organisationBuildings) { }

    public OrganisationBuildings(params OrganisationBuilding[] organisationBuildings) : base(organisationBuildings) { }

    public bool AlreadyHasTheSameOrganisationAndBuildingInTheSamePeriod(OrganisationBuilding organisationBuilding)
        => Except(organisationBuilding.OrganisationBuildingId)
            .WithBuilding(organisationBuilding.BuildingId)
            .OverlappingWith(organisationBuilding.Validity)
            .Any();

    public bool OrganisationAlreadyHasAMainBuildingInTheSamePeriod(OrganisationBuilding organisationBuilding)
        => Except(organisationBuilding.OrganisationBuildingId)
            .OnlyMainBuildings()
            .OverlappingWith(organisationBuilding.Validity)
            .Any();

    public OrganisationBuildings Except(Guid organisationBuildingId)
        => new(
            this.Where(ob => ob.OrganisationBuildingId != organisationBuildingId));

    public OrganisationBuildings WithBuilding(Guid buildingId)
        => new(
            this.Where(ob => ob.BuildingId == buildingId));

    public OrganisationBuildings OverlappingWith(Period validity)
        => new(
            this.Where(ob => ob.Validity.OverlapsWith(validity)));

    public OrganisationBuildings OnlyMainBuildings()
        => new(
            this.Where(ob => ob.IsMainBuilding));

    public OrganisationBuilding? TryFindMainOrganisationBuildingValidFor(DateTime date, Guid buildingId)
        => this
            .Where(building => building.BuildingId == buildingId)
            .Where(building => building.IsMainBuilding)
            .SingleOrDefault(building => building.IsValid(date));

    public OrganisationBuilding? TryFindMainOrganisationBuildingValidFor(DateTime date)
        => this
            .Where(building => building.IsMainBuilding)
            .SingleOrDefault(building => building.IsValid(date));
}