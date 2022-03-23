namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using Organisation;

    public class OrganisationBuildingTestDataBuilder
    {
        private Period _validity;
        private Guid _organisationBuildingId;
        private Guid _organisationId;
        private Guid _buildingId;
        private bool _isMainBuilding;

        public OrganisationBuildingTestDataBuilder()
        {
            _organisationBuildingId = Guid.NewGuid();
            _organisationId = Guid.NewGuid();
            _buildingId = Guid.NewGuid();
            _isMainBuilding = false;
            _validity = new Period();
        }

        public OrganisationBuildingTestDataBuilder(OrganisationBuildingTestDataBuilder dataBuilder)
        {
            _organisationBuildingId = dataBuilder._organisationBuildingId;
            _organisationId = dataBuilder._organisationId;
            _buildingId = dataBuilder._buildingId;
            _isMainBuilding = dataBuilder._isMainBuilding;
            _validity = dataBuilder._validity;
        }

        public OrganisationBuildingTestDataBuilder WithOrganisationBuildingId(Guid organisationBuildingId)
        {
            _organisationBuildingId = organisationBuildingId;
            return this;
        }

        public OrganisationBuildingTestDataBuilder WithOrganisationId(Guid organisationId)
        {
            _organisationId = organisationId;
            return this;
        }

        public OrganisationBuildingTestDataBuilder WithBuildingId(Guid buildingId)
        {
            _buildingId = buildingId;
            return this;
        }

        public OrganisationBuildingTestDataBuilder WithMainBuilding(bool isMainBuilding)
        {
            _isMainBuilding = isMainBuilding;
            return this;
        }

        public OrganisationBuildingTestDataBuilder WithValidity(Period validity)
        {
            _validity = validity;
            return this;
        }

        public OrganisationBuilding Build()
            => new(
                _organisationBuildingId,
                _organisationId,
                _buildingId,
                _buildingId.ToString(),
                _isMainBuilding,
                _validity);

        public static implicit operator OrganisationBuilding(OrganisationBuildingTestDataBuilder dataBuilder)
            => dataBuilder.Build();
    }
}
