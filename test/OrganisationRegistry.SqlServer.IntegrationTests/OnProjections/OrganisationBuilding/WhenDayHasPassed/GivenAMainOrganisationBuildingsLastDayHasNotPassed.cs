namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationBuilding.WhenDayHasPassed
{
    using System;
    using System.Collections.Generic;
    using Autofac.Features.OwnedInstances;
    using Day.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Organisation.ScheduledActions.Building;
    using TestBases;
    using Tests.Shared;
    using OrganisationRegistry.Building.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class WhenAMainOrganisationBuildingsLastDayHasNotPassed : ReactionCommandsTestBase<ActiveOrganisationBuildingListView, DayHasPassed>
    {
        private readonly Guid _organisationId;
        private readonly Guid _buildingAId;
        private readonly Guid _buildingBId;
        private readonly Guid _organisationBuildingAId;
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

        public WhenAMainOrganisationBuildingsLastDayHasNotPassed(SqlServerFixture fixture) : base(fixture)
        {
            _organisationId = Guid.NewGuid();
            _buildingAId = Guid.NewGuid();
            _buildingBId = Guid.NewGuid();

            _organisationBuildingAId = Guid.NewGuid();
        }

        protected override ActiveOrganisationBuildingListView BuildReactionHandler()
        {
            return new ActiveOrganisationBuildingListView(
                new Mock<ILogger<ActiveOrganisationBuildingListView>>().Object,
                () => new Owned<OrganisationRegistryContext>(new OrganisationRegistryTransactionalContext(SqlConnection, Transaction), this),
                null,
                new DateTimeProvider());
        }

        protected override IEnumerable<IEvent> Given()
        {
            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", _sequentialOvoNumberGenerator.GenerateNumber(), "K&G", "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null),
                new BuildingCreated(_buildingAId, "Gebouw A", 12345),
                new BuildingCreated(_buildingBId, "Gebouw B", 12345),
                new OrganisationBuildingAdded(_organisationId, _organisationBuildingAId, _buildingAId, "Gebouw A", true, DateTime.Today, DateTime.Today.AddDays(1))
            };
        }

        protected override DayHasPassed When()
        {
            return new DayHasPassed(Guid.NewGuid(), DateTime.Today);
        }

        protected override int ExpectedNumberOfCommands => 0;
    }
}
