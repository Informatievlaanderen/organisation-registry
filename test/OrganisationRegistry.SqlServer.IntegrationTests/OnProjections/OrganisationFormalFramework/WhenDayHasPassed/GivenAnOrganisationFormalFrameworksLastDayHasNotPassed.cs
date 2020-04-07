namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationFormalFramework.WhenDayHasPassed
{
    using System;
    using System.Collections.Generic;
    using Autofac.Features.OwnedInstances;
    using Day.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Organisation.ScheduledActions.FormalFramework;
    using TestBases;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using UnitTests;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class GivenAnOrganisationFormalFrameworksLastDayHasNotPassed : ReactionCommandsTestBase<ActiveOrganisationFormalFrameworkListView, DayHasPassed>
    {
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

        public GivenAnOrganisationFormalFrameworksLastDayHasNotPassed(SqlServerFixture fixture) : base(fixture)
        {
            Guid.NewGuid();
        }

        protected override ActiveOrganisationFormalFrameworkListView BuildReactionHandler(Func<OrganisationRegistryContext> context)
        {
            return new ActiveOrganisationFormalFrameworkListView(
                Mock.Of<ILogger<ActiveOrganisationFormalFrameworkListView>>(),
                () => new Owned<OrganisationRegistryContext>(context(), this),
                null,
                new DateTimeProviderStub(DateTime.Now),
                (connection, transaction) => context());
        }

        protected override IEnumerable<IEvent> Given()
        {
            var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedTestDataBuilder();
            var formalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
            var parentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var childOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var organisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(childOrganisationCreated.Id, formalFrameworkCreated.Id, parentOrganisationCreated.Id)
                .WithValidity(DateTime.Today, DateTime.Today.AddDays(2));
            return new List<IEvent>
            {
                parentOrganisationCreated.Build(),
                childOrganisationCreated.Build(),
                formalFrameworkCategoryCreated.Build(),
                formalFrameworkCreated.Build(),
                organisationFormalFrameworkAdded.Build(),
            };
        }

        protected override DayHasPassed When()
        {
            return new DayHasPassed(Guid.NewGuid(), DateTime.Today);
        }

        protected override int ExpectedNumberOfCommands => 0;
    }
}
