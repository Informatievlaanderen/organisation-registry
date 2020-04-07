namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationBuilding.WhenDayHasPassed
{
    using System;
    using System.Collections.Generic;
    using Day.Events;
    using FluentAssertions;
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
    public class GivenMultipleMainOrganisationBuildingsLastDayHavePassed : ReactionCommandsTestBase<ActiveOrganisationBuildingListView, DayHasPassed>
    {
        private OrganisationId _organisationAId;
        private OrganisationId _organisationBId;
        private Guid _buildingAId;
        private Guid _buildingBId;
        private Guid _organisationBuildingAId;
        private Guid _organisationBuildingBId;
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

        public GivenMultipleMainOrganisationBuildingsLastDayHavePassed(SqlServerFixture fixture) : base()
        {
        }

        protected override ActiveOrganisationBuildingListView BuildReactionHandler(IContextFactory contextFactory)
        {
            return new ActiveOrganisationBuildingListView(
                new Mock<ILogger<ActiveOrganisationBuildingListView>>().Object,
                null,
                new DateTimeProvider(),
                contextFactory);
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationAId = new OrganisationId(Guid.NewGuid());
            _organisationBId = new OrganisationId(Guid.NewGuid());
            _buildingAId = Guid.NewGuid();
            _buildingBId = Guid.NewGuid();

            _organisationBuildingAId = Guid.NewGuid();
            _organisationBuildingBId = Guid.NewGuid();

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationAId, "Kind en Gezin", _sequentialOvoNumberGenerator.GenerateNumber(), "K&G", "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null),
                new OrganisationCreated(_organisationBId, "Stedenbouw", _sequentialOvoNumberGenerator.GenerateNumber(), "SB", "Schatjes van stadjes", new List<Purpose>(), false, null, null),
                new OrganisationCreated(Guid.NewGuid(), "Gebouwenregister", _sequentialOvoNumberGenerator.GenerateNumber(), "GR", "Daar kan je op bouwen", new List<Purpose>(), false, null, null),
                new BuildingCreated(_buildingAId, "Gebouw A789", 12345),
                new BuildingCreated(_buildingBId, "Gebouw B789", 12345),
                new OrganisationBuildingAdded(_organisationAId, _organisationBuildingAId, _buildingAId, "Gebouw A789", true, DateTime.Today, DateTime.Today),
                new OrganisationBuildingAdded(_organisationBId, _organisationBuildingBId, _buildingBId, "Gebouw B789", true, DateTime.Today, DateTime.Today),
                new MainBuildingAssignedToOrganisation(_organisationAId, _buildingAId, _organisationBuildingAId),
                new MainBuildingAssignedToOrganisation(_organisationBId, _buildingBId, _organisationBuildingBId)
            };
        }

        protected override DayHasPassed When()
        {
            return new DayHasPassed(Guid.NewGuid(), DateTime.Today);
        }

        protected override int ExpectedNumberOfCommands => 2;

        [Fact]
        public void UpdateMainBuildingForEachOfThoseBuildingsOrganisationIsFired()
        {
            Commands.Should().Contain(new UpdateMainBuilding(_organisationAId));
            Commands.Should().Contain(new UpdateMainBuilding(_organisationBId));
        }
    }
}
