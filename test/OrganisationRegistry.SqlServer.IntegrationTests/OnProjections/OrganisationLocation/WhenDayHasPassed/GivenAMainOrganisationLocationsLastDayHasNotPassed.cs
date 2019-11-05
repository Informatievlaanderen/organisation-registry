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
    public class WhenAMainOrganisationLocationsLastDayHasNotPassed : ReactionCommandsTestBase<ActiveOrganisationLocationListView, DayHasPassed>
    {
        private readonly Guid _organisationId;
        private readonly Guid _locationAId;
        private readonly Guid _organisationLocationAId;
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

        public WhenAMainOrganisationLocationsLastDayHasNotPassed(SqlServerFixture fixture) : base(fixture)
        {
            _organisationId = Guid.NewGuid();
            _locationAId = Guid.NewGuid();

            _organisationLocationAId = Guid.NewGuid();
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
            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", _sequentialOvoNumberGenerator.GenerateNumber(), "K&G", "Kindjes en gezinnetjes", new List<Purpose>(),  false, null, null),
                new LocationCreated(_locationAId, "12345", "Boudewijn 1 laan 32, 1000 Brussel", "Boudewijn 1 laan 32", "1000", "Brussel", "Belgie"),
                new OrganisationLocationAdded(_organisationId, _organisationLocationAId, _locationAId, "Boudewijn 1 laan 32, 1000 Brussel", true, null, null, DateTime.Today, DateTime.Today.AddDays(1)),
                new MainLocationAssignedToOrganisation(_organisationId, _locationAId, _organisationLocationAId),
            };
        }

        protected override DayHasPassed When()
        {
            return new DayHasPassed(Guid.NewGuid(), DateTime.Today);
        }

        protected override int ExpectedNumberOfCommands => 0;
    }
}
