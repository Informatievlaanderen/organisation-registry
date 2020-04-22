namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationParent.WhenDayHasPassed
{
    using System;
    using System.Collections.Generic;
    using Day.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Organisation.ScheduledActions.Parent;
    using TestBases;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class GivenARegularOrganisationParentsLastDayHasPassed : ReactionCommandsTestBase<ActiveOrganisationParentListView, DayHasPassed>
    {
        private OrganisationCreatedTestDataBuilder _organisationCreated;
        private OrganisationCreatedTestDataBuilder _parentOrganisationCreated;

        public GivenARegularOrganisationParentsLastDayHasPassed(SqlServerFixture fixture) : base()
        {
        }

        protected override ActiveOrganisationParentListView BuildReactionHandler(IContextFactory contextFactory)
        {
            return new ActiveOrganisationParentListView(
                new Mock<ILogger<ActiveOrganisationParentListView>>().Object,
                null,
                new DateTimeProvider(),
                contextFactory);}

        protected override IEnumerable<IEvent> Given()
        {
            var sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
            _organisationCreated = new OrganisationCreatedTestDataBuilder(sequentialOvoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedTestDataBuilder(sequentialOvoNumberGenerator);

            var organisationParentAdded = new OrganisationParentAddedTestDataBuilder(_organisationCreated.Id, _parentOrganisationCreated.Id)
                .WithValidity(DateTime.Today, DateTime.Today);

            return new List<IEvent>
            {
                _organisationCreated.Build(),
                _parentOrganisationCreated.Build(),
                organisationParentAdded.Build()
            };
        }

        protected override DayHasPassed When()
        {
            return new DayHasPassed(Guid.NewGuid(), DateTime.Today);
        }

        protected override int ExpectedNumberOfCommands => 0;
    }
}
