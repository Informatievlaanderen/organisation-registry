namespace OrganisationRegistry.SqlServer.IntegrationTests.OnEventStore.OrganisationBuilding.WhenDayHasPassed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Day.Events;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using Polly;
    using TestBases;
    using Tests.Shared;
    using OrganisationRegistry.Building.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;

    [Collection(EventStoreTestsCollection.Name)]
    public class GivenAMainOrganisationBuildingsLastDayHasPassedWithIntegration : EventStoreIntegrationTestBase<DayHasPassed>
    {
        private Guid _organisationAId;
        private Guid _organisationBId;
        private Guid _buildingAId;
        private Guid _buildingBId;
        private Guid _organisationBuildingAId;
        private Guid _organisationBuildingBId;
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        private static readonly DateTime DateTime = DateTime.Today;

        public GivenAMainOrganisationBuildingsLastDayHasPassedWithIntegration(EventStoreSqlServerFixture fixture) :
            base(new DateTimeProviderStub(DateTime), fixture)
        {
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationAId = Guid.NewGuid();
            _organisationBId = Guid.NewGuid();
            _buildingAId = Guid.NewGuid();
            _buildingBId = Guid.NewGuid();

            _organisationBuildingAId = Guid.NewGuid();
            _organisationBuildingBId = Guid.NewGuid();

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationAId, "Kind en Gezin", _sequentialOvoNumberGenerator.GenerateNumber(), "K&G", "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null),
                new OrganisationCreated(_organisationBId, "Stedenbouw", _sequentialOvoNumberGenerator.GenerateNumber(), "SB", "Schatjes van stadjes", new List<Purpose>(), false, null, null),
                new BuildingCreated(_buildingAId, "Gebouw A", null),
                new BuildingCreated(_buildingBId, "Gebouw B", null),
                new OrganisationBuildingAdded(_organisationAId, _organisationBuildingAId, _buildingAId, "Gebouw A", true, DateTime.Today, DateTime.Today),
                new MainBuildingAssignedToOrganisation(_organisationAId, _buildingAId, _organisationBuildingAId),
                new OrganisationBuildingAdded(_organisationBId, _organisationBuildingBId, _buildingBId, "Gebouw B", true, DateTime.Today, DateTime.Today),
                new MainBuildingAssignedToOrganisation(_organisationBId, _buildingBId, _organisationBuildingBId)
            };
        }

        protected override DayHasPassed When()
        {
            ((DateTimeProviderStub) DateTimeProvider).StubDate = DateTimeProvider.Today.AddDays(1);
            return new DayHasPassed(Guid.NewGuid(), DateTime);
        }

        [Fact]
        public async Task MainBuildingIsCleared()
        {
            var waitAndRetry = Policy.Handle<AssertionFailedException>().RetryAsync(5);

            await waitAndRetry.ExecuteAsync(async () =>
            {
                await Task.Delay(1000);
                var organisationDetailItems = AutofacServiceProvider.GetService<OrganisationRegistryContext>().OrganisationDetail.ToList();
                organisationDetailItems[0].MainBuildingId.Should().BeNull();
                organisationDetailItems[0].MainBuildingName.Should().BeNullOrEmpty();
                organisationDetailItems[1].MainBuildingId.Should().BeNull();
                organisationDetailItems[1].MainBuildingName.Should().BeNullOrEmpty();
            });
        }
    }
}
