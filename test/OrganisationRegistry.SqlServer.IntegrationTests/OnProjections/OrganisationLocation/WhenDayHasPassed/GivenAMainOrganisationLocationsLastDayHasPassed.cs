namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationLocation.WhenDayHasPassed
{
    using System;
    using System.Collections.Generic;
    using Autofac.Features.OwnedInstances;
    using Day.Events;
    using FluentAssertions;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OnEventStore;
    using Organisation;
    using Organisation.ScheduledActions.Location;
    using TestBases;
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Location.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class GivenAMainOrganisationLocationsLastDayHasPassed : ReactionCommandsTestBase<ActiveOrganisationLocationListView, DayHasPassed>
    {
        private OrganisationId _organisationAId;
        private OrganisationId _organisationBId;
        private Guid _locationAId;
        private Guid _locationBId;
        private Guid _organisationLocationAId;
        private Guid _organisationLocationBId;
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        private DateTimeProviderStub _dateTimeProviderStub;

        public GivenAMainOrganisationLocationsLastDayHasPassed(SqlServerFixture fixture) : base(fixture)
        {
        }

        protected override ActiveOrganisationLocationListView BuildReactionHandler()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Today.AddDays(-1));
            return new ActiveOrganisationLocationListView(
                new Mock<ILogger<ActiveOrganisationLocationListView>>().Object,
                () => new Owned<OrganisationRegistryContext>(new OrganisationRegistryTransactionalContext(SqlConnection, Transaction), this),
                null,
                _dateTimeProviderStub);
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationAId = new OrganisationId(Guid.NewGuid());
            _organisationBId = new OrganisationId(Guid.NewGuid());
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
                new OrganisationLocationAdded(_organisationAId, _organisationLocationAId, _locationAId, "Boudewijn 1 laan 32, 1000 Brussel", true, null, null, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1)),
                new OrganisationLocationAdded(_organisationBId, _organisationLocationBId, _locationBId, "Boudewijn 2 laan 32, 1000 Brussel", true, null, null, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1)),
                new MainLocationAssignedToOrganisation(_organisationAId, _locationAId, _organisationLocationAId),
                new MainLocationAssignedToOrganisation(_organisationBId, _locationBId, _organisationLocationBId)
            };
        }

        protected override DayHasPassed When()
        {
            _dateTimeProviderStub.StubDate = DateTime.Today;
            return new DayHasPassed(Guid.NewGuid(), DateTime.Today.AddDays(-1));
        }

        protected override int ExpectedNumberOfCommands => 2;

        [Fact]
        public void UpdateMainLocationForEachOrganisationIsFired()
        {
            Commands.Should().Contain(new UpdateMainLocation(_organisationAId));
            Commands.Should().Contain(new UpdateMainLocation(_organisationBId));
        }
    }
}
