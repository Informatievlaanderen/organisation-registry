namespace OrganisationRegistry.Organisation
{
    using System.Linq;
    using System.Text;
    using Events;

    public partial class Organisation
    {
        // Bugfix: Clear Main Location
        // ---------------------------
        // A bug was present that did not clear the main location if:
        // - validity went from valid to invalid
        // - AND isMainLocation went from true to false.

        // This fixes the main location

        public string Bugfix_20170330_ClearMainLocationFix(IDateTimeProvider dateTimeProvider)
        {
            var bugFixResult = new StringBuilder();

            Bugfix_20170330_ClearMainLocationFix_FixLocations(dateTimeProvider, bugFixResult);
            Bugfix_20170330_ClearMainBuildingFix_FixBuildings(dateTimeProvider, bugFixResult);

            return bugFixResult.ToString();
        }

        private void Bugfix_20170330_ClearMainLocationFix_FixLocations(IDateTimeProvider dateTimeProvider, StringBuilder bugFixResult)
        {
            if (_mainOrganisationLocation != null && _mainOrganisationLocation.Validity.OverlapsWith(dateTimeProvider.Today))
                return; // bestaat en overlapt met vandaag.

            if (_mainOrganisationLocation != null)
            {
                bugFixResult.AppendLine($"Ongeldige main location gevonden! {_mainOrganisationLocation.FormattedAddress}, geldig op {_mainOrganisationLocation.Validity}.");
                ApplyChange(new MainLocationClearedFromOrganisation(Id, _mainOrganisationLocation.LocationId)); // bestaat en overlapt niet met vandaag.
            }

            var mainLocation = _organisationLocations.SingleOrDefault(location => location.IsMainLocation && location.Validity.OverlapsWith(dateTimeProvider.Today));
            if (mainLocation != null)
            {
                bugFixResult.AppendLine($"Nieuwe main location gevonden! {mainLocation.FormattedAddress}, geldig op {mainLocation.Validity}.");
                ApplyChange(new MainLocationAssignedToOrganisation(Id, mainLocation.LocationId, mainLocation.OrganisationLocationId)); // apply degene die vandaag geldig is.
            }
        }

        private void Bugfix_20170330_ClearMainBuildingFix_FixBuildings(IDateTimeProvider dateTimeProvider, StringBuilder bugFixResult)
        {
            if (_mainOrganisationBuilding != null && _mainOrganisationBuilding.Validity.OverlapsWith(dateTimeProvider.Today))
                return; // bestaat en overlapt met vandaag.

            if (_mainOrganisationBuilding != null)
            {
                bugFixResult.AppendLine($"Ongeldige main building gevonden! {_mainOrganisationBuilding.BuildingName}, geldig op {_mainOrganisationBuilding.Validity}.");
                ApplyChange(new MainBuildingClearedFromOrganisation(Id, _mainOrganisationBuilding.BuildingId)); // bestaat en overlapt niet met vandaag.
            }

            var mainBuilding = _organisationBuildings.SingleOrDefault(building => building.IsMainBuilding && building.Validity.OverlapsWith(dateTimeProvider.Today));
            if (mainBuilding != null)
            {
                bugFixResult.AppendLine($"Nieuwe main building gevonden! {mainBuilding.BuildingName}, geldig op {mainBuilding.Validity}.");
                ApplyChange(new MainBuildingAssignedToOrganisation(Id, mainBuilding.BuildingId, mainBuilding.OrganisationBuildingId)); // apply degene die vandaag geldig is.
            }
        }
    }
}
