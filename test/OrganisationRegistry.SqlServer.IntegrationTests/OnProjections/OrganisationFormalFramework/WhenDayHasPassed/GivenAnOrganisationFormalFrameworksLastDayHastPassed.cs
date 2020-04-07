namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationFormalFramework.WhenDayHasPassed
{
    using System;
    using System.Collections.Generic;
    using Day.Events;
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
    public class GivenAnOrganisationFormalFrameworksLastDayHasPassed : ReactionCommandsTestBase<ActiveOrganisationFormalFrameworkListView, DayHasPassed>
    {
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        private DateTimeProviderStub _dateTimeProviderStub;

        public GivenAnOrganisationFormalFrameworksLastDayHasPassed(SqlServerFixture fixture) : base()
        {

        }

        protected override ActiveOrganisationFormalFrameworkListView BuildReactionHandler(IContextFactory contextFactory)
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Today.AddDays(-1));
            return new ActiveOrganisationFormalFrameworkListView(
                Mock.Of<ILogger<ActiveOrganisationFormalFrameworkListView>>(),
                null,
                _dateTimeProviderStub,
                contextFactory);
        }

        protected override IEnumerable<IEvent> Given()
        {
            var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedTestDataBuilder();
            var formalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
            var parentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var anotherParentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var childOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);

            var organisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                        childOrganisationCreated.Id,
                        formalFrameworkCreated.Id,
                        parentOrganisationCreated.Id)
                    .WithValidity(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1));

            var anotherOrganisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                        parentOrganisationCreated.Id,
                        formalFrameworkCreated.Id,
                        parentOrganisationCreated.Id)
                    .WithValidity(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1));

            var formalFrameworkAssigned =
                new FormalFrameworkAssignedToOrganisationTestDataBuilder(
                    organisationFormalFrameworkAdded.OrganisationFormalFrameworkId,
                    organisationFormalFrameworkAdded.FormalFrameworkId,
                    organisationFormalFrameworkAdded.OrganisationId,
                    organisationFormalFrameworkAdded.ParentOrganisationId);

            return new List<IEvent>
            {
                parentOrganisationCreated.Build(),
                anotherParentOrganisationCreated.Build(),
                childOrganisationCreated.Build(),
                formalFrameworkCategoryCreated.Build(),
                formalFrameworkCreated.Build(),
                organisationFormalFrameworkAdded.Build(),
                anotherOrganisationFormalFrameworkAdded.Build(),
                formalFrameworkAssigned.Build()
            };
        }

        protected override DayHasPassed When()
        {
            _dateTimeProviderStub.StubDate = DateTime.Today;
            return new DayHasPassed(Guid.NewGuid(), DateTime.Today.AddDays(-1));
        }

        protected override int ExpectedNumberOfCommands => 1;
    }
}
