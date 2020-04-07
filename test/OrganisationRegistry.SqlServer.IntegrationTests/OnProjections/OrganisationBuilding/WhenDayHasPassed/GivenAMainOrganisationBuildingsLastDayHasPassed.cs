namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationBuilding.WhenDayHasPassed
{
    using System;
    using System.Collections.Generic;
    using Autofac.Features.OwnedInstances;
    using Day.Events;
    using FluentAssertions;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Organisation.ScheduledActions.Building;
    using TestBases;
    using Tests.Shared;
    using OrganisationRegistry.Building.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class GivenAMainOrganisationBuildingsLastDayHasPassed : ReactionCommandsTestBase<ActiveOrganisationBuildingListView, DayHasPassed>
    {
        private OrganisationId _organisationId;
        private Guid _buildingAId;
        private Guid _organisationBuildingAId;

        public GivenAMainOrganisationBuildingsLastDayHasPassed(SqlServerFixture fixture) : base(fixture)
        {
        }

        protected override ActiveOrganisationBuildingListView BuildReactionHandler(Func<OrganisationRegistryContext> context)
        {
            return new ActiveOrganisationBuildingListView(
                new Mock<ILogger<ActiveOrganisationBuildingListView>>().Object,
                () => new Owned<OrganisationRegistryContext>(context(), this),
                null,
                new DateTimeProvider(),
                (connection, transaction) => context());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationId = new OrganisationId(Guid.NewGuid());
            _buildingAId = Guid.NewGuid();

            _organisationBuildingAId = Guid.NewGuid();

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", new SequentialOvoNumberGenerator().GenerateNumber(), "K&G", "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null),
                new BuildingCreated(_buildingAId, "Gebouw A123456", 12345),
                new OrganisationBuildingAdded(_organisationId, _organisationBuildingAId, _buildingAId, "Gebouw A123456", true, DateTime.Today, DateTime.Today),
                new MainBuildingAssignedToOrganisation(_organisationId, _buildingAId, _organisationBuildingAId)
            };
        }

        protected override DayHasPassed When()
        {
            return new DayHasPassed(Guid.NewGuid(), DateTime.Today);
        }

        protected override int ExpectedNumberOfCommands => 1;

        [Fact]
        public void UpdateMainBuildingForThatOrganisationIsFired()
        {
            Commands.Should().Contain(new UpdateMainBuilding(_organisationId));
        }
    }
}
