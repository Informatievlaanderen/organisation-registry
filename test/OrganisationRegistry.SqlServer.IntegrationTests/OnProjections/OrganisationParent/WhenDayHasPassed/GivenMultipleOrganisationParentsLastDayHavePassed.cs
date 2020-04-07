namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationParent.WhenDayHasPassed
{
    using System;
    using System.Collections.Generic;
    using Autofac.Features.OwnedInstances;
    using Day.Events;
    using FluentAssertions;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Organisation.ScheduledActions.Parent;
    using TestBases;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class GivenMultipleOrganisationParentsLastDayHavePassed : ReactionCommandsTestBase<ActiveOrganisationParentListView, DayHasPassed>
    {
        private OrganisationCreatedTestDataBuilder _organisationACreated;
        private OrganisationCreatedTestDataBuilder _organisationBCreated;

        public GivenMultipleOrganisationParentsLastDayHavePassed(SqlServerFixture fixture) : base(fixture)
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
            _organisationACreated = new OrganisationCreatedTestDataBuilder(sequentialOvoNumberGenerator);
            _organisationBCreated = new OrganisationCreatedTestDataBuilder(sequentialOvoNumberGenerator);
            var parentOrganisationCreated = new OrganisationCreatedTestDataBuilder(sequentialOvoNumberGenerator);

            var organisationParentAddedToA = new OrganisationParentAddedTestDataBuilder(_organisationACreated.Id, parentOrganisationCreated.Id)
                .WithValidity(DateTime.Today, DateTime.Today);

            var organisationParentAddedToB = new OrganisationParentAddedTestDataBuilder(_organisationBCreated.Id, parentOrganisationCreated.Id)
                .WithValidity(DateTime.Today, DateTime.Today);

            return new List<IEvent>
            {
                _organisationACreated.Build(),
                _organisationBCreated.Build(),
                parentOrganisationCreated.Build(),
                organisationParentAddedToA.Build(),
                organisationParentAddedToB.Build(),
                new ParentAssignedToOrganisation(_organisationACreated.Id, parentOrganisationCreated.Id, organisationParentAddedToA.OrganisationOrganisationParentId),
                new ParentAssignedToOrganisation(_organisationBCreated.Id, parentOrganisationCreated.Id, organisationParentAddedToB.OrganisationOrganisationParentId)
            };
        }

        protected override DayHasPassed When()
        {
            return new DayHasPassed(Guid.NewGuid(), DateTime.Today);
        }

        protected override int ExpectedNumberOfCommands => 2;

        [Fact]
        public void UpdateCurrentOrganisationParentForEachOfThoseBuildingsOrganisationIsFired()
        {
            Commands.Should().Contain(new UpdateCurrentOrganisationParent(_organisationACreated.Id));
            Commands.Should().Contain(new UpdateCurrentOrganisationParent(_organisationBCreated.Id));
        }
    }
}
