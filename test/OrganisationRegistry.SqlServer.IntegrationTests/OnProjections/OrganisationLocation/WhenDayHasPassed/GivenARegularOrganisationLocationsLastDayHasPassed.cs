namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationLocation.WhenDayHasPassed
{
    using System;
    using System.Collections.Generic;
    using Autofac.Features.OwnedInstances;
    using Day.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Organisation.ScheduledActions.Location;
    using TestBases;
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Location.Events;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class GivenARegularOrganisationLocationsLastDayHasPassed : ReactionCommandsTestBase<ActiveOrganisationLocationListView, DayHasPassed>
    {
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

        public GivenARegularOrganisationLocationsLastDayHasPassed(SqlServerFixture fixture) : base(fixture)
        {
        }

        protected override ActiveOrganisationLocationListView BuildReactionHandler()
        {
            return new ActiveOrganisationLocationListView(
                new Mock<ILogger<ActiveOrganisationLocationListView>>().Object,
                () => new Owned<OrganisationRegistryContext>(new OrganisationRegistryTransactionalContext(SqlConnection, Transaction), this),
                null,
                new DateTimeProvider());
        }

        protected override IEnumerable<IEvent> Given()
        {
            var organisationId = Guid.NewGuid();
            var locationAId = Guid.NewGuid();

            return new List<IEvent>
            {
                new OrganisationCreated(organisationId, "Kind en Gezin", _sequentialOvoNumberGenerator.GenerateNumber(), "K&G", "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null),
                new LocationCreated(locationAId, "12345", "Albert 1 laan 32, 1000 Brussel", "Albert 1 laan 32", "1000", "Brussel", "Belgie"),
                new OrganisationLocationAdded(organisationId, Guid.NewGuid(), locationAId, "Albert 1 laan 32, 1000 Brussel", false, null, null, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1))
            };
        }

        protected override DayHasPassed When()
        {
            return new DayHasPassed(Guid.NewGuid(), DateTime.Today);
        }

        protected override int ExpectedNumberOfCommands => 0;
    }
}
