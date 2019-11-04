namespace OrganisationRegistry.Organisation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OrganisationBuildings : List<OrganisationBuilding>
    {
        public OrganisationBuildings() { }

        public OrganisationBuildings(IEnumerable<OrganisationBuilding> organisationBuildings) : base(organisationBuildings) { }

        public OrganisationBuildings(params OrganisationBuilding[] organisationBuildings) : base(organisationBuildings) { }

        public bool AlreadyHasTheSameOrganisationAndBuildingInTheSamePeriod(OrganisationBuilding organisationBuilding)
        {
            return this
                .Except(organisationBuilding.OrganisationBuildingId)
                .WithBuilding(organisationBuilding.BuildingId)
                .OverlappingWith(organisationBuilding.Validity)
                .Any();
        }

        public bool OrganisationAlreadyHasAMainBuildingInTheSamePeriod(OrganisationBuilding organisationBuilding)
        {
            return this
                .Except(organisationBuilding.OrganisationBuildingId)
                .OnlyMainBuildings()
                .OverlappingWith(organisationBuilding.Validity)
                .Any();
        }

        public OrganisationBuildings Except(Guid organisationBuildingId)
        {
            return new OrganisationBuildings(
                this.Where(ob => ob.OrganisationBuildingId != organisationBuildingId));
        }

        public OrganisationBuildings WithBuilding(Guid buildingId)
        {
            return new OrganisationBuildings(
                this.Where(ob => ob.BuildingId == buildingId));
        }

        public OrganisationBuildings OverlappingWith(Period validity)
        {
            return new OrganisationBuildings(
                this.Where(ob => ob.Validity.OverlapsWith(validity)));
        }

        public OrganisationBuildings OnlyMainBuildings()
        {
            return new OrganisationBuildings(
                this.Where(ob => ob.IsMainBuilding));
        }

        public OrganisationBuilding TryFindMainOrganisationBuildingValidFor(DateTime date, Guid buildingId)
        {
            return this
                .Where(building => building.BuildingId == buildingId)
                .Where(building => building.IsMainBuilding)
                .SingleOrDefault(building => building.IsValid(date));
        }

        public OrganisationBuilding TryFindMainOrganisationBuildingValidFor(DateTime date)
        {
            return this
                .Where(building => building.IsMainBuilding)
                .SingleOrDefault(building => building.IsValid(date));
        }
    }
}
