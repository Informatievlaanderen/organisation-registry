namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationParent.WhenDayHasPassed
{
    using System;
    using System.Collections.Generic;
    using Autofac.Features.OwnedInstances;
    using Day.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Organisation.ScheduledActions.Parent;
    using TestBases;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class WhenACurrentOrganisationParentsLastDayHasNotPassed : ReactionCommandsTestBase<ActiveOrganisationParentListView, DayHasPassed>
    {
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        private OrganisationCreatedTestDataBuilder _organisationCreated;
        private OrganisationCreatedTestDataBuilder _parentOrganisationCreated;

        public WhenACurrentOrganisationParentsLastDayHasNotPassed(SqlServerFixture fixture) : base(fixture)
        {
        }

        protected override ActiveOrganisationParentListView BuildReactionHandler(Func<OrganisationRegistryContext> context)
        {
            return new ActiveOrganisationParentListView(
                new Mock<ILogger<ActiveOrganisationParentListView>>().Object,
                () => new Owned<OrganisationRegistryContext>(context(), this),
                null,
                new DateTimeProvider(),
                (connection, transaction) => context());}

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
                organisationParentAdded.Build(),
                new ParentAssignedToOrganisation(_organisationCreated.Id, _parentOrganisationCreated.Id, organisationParentAdded.OrganisationOrganisationParentId)
            };
        }

        protected override DayHasPassed When()
        {
            return new DayHasPassed(Guid.NewGuid(), DateTime.Today.AddDays(-1));
        }

        protected override int ExpectedNumberOfCommands => 0;
    }
}
