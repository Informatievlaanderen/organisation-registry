namespace OrganisationRegistry.SqlServer.IntegrationTests.OnEventStore.OrganisationLocation.WhenDayHasPassed
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
    using Microsoft.Extensions.Logging;
    using Polly;
    using TestBases;
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Location.Events;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;

    [Collection(EventStoreTestsCollection.Name)]
    public class GivenAMainOrganisationLocationsLastDayHasPassedWithIntegration : EventStoreIntegrationTestBase<DayHasPassed>
    {
        private Guid _organisationAId;
        private Guid _organisationBId;
        private Guid _locationAId;
        private Guid _locationBId;
        private Guid _organisationLocationAId;
        private Guid _organisationLocationBId;
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        private static readonly DateTime Date = DateTime.Today;

        public GivenAMainOrganisationLocationsLastDayHasPassedWithIntegration(EventStoreSqlServerFixture fixture) :
            base(new DateTimeProviderStub(Date), fixture, new LoggerFactory())
        {
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationAId = Guid.NewGuid();
            _organisationBId = Guid.NewGuid();
            _locationAId = Guid.NewGuid();
            _locationBId = Guid.NewGuid();

            _organisationLocationAId = Guid.NewGuid();
            _organisationLocationBId = Guid.NewGuid();

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationAId, "Kind en Gezin", _sequentialOvoNumberGenerator.GenerateNumber(), "K&G", "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null),
                new OrganisationCreated(_organisationBId, "Stedenbouw", _sequentialOvoNumberGenerator.GenerateNumber(), "SB", "Schatjes van stadjes", new List<Purpose>(), false, null, null),
                new LocationCreated(_locationAId, "12345", "Boudewijn 1 laan 32, 1000 Brussel", "Boudewijn 1 laan 32", "1000", "Brussel", "Belgie"),
                new LocationCreated(_locationBId, "12345", "Boudewijn 2 laan 32, 1000 Brussel", "Boudewijn 1 laan 32", "1000", "Brussel", "Belgie"),
                new OrganisationLocationAdded(_organisationAId, _organisationLocationAId, _locationAId, "Gebouw A", true, null, null, DateTime.Today, DateTime.Today),
                new MainLocationAssignedToOrganisation(_organisationAId, _locationAId, _organisationLocationAId),
                new OrganisationLocationAdded(_organisationBId, _organisationLocationBId, _locationBId, "Gebouw B", true, null, null, DateTime.Today, DateTime.Today),
                new MainLocationAssignedToOrganisation(_organisationBId, _locationBId, _organisationLocationBId)
            };
        }

        protected override DayHasPassed When()
        {
            ((DateTimeProviderStub) DateTimeProvider).StubDate = Date.AddDays(1);
            return new DayHasPassed(Guid.NewGuid(), Date);
        }

        [Fact]
        public async Task MainLocationIsCleared()
        {
            var waitAndRetry = Policy.Handle<AssertionFailedException>().RetryAsync(5);

            await waitAndRetry.ExecuteAsync(async () =>
            {
                await Task.Delay(1000);
                var organisationDetailItems = AutofacServiceProvider.GetService<OrganisationRegistryContext>().OrganisationDetail.ToList();
                organisationDetailItems[0].MainLocationId.Should().BeNull();
                organisationDetailItems[0].MainLocationName.Should().BeNullOrEmpty();
                organisationDetailItems[1].MainLocationId.Should().BeNull();
                organisationDetailItems[1].MainLocationName.Should().BeNullOrEmpty();
            });
        }
    }
}
