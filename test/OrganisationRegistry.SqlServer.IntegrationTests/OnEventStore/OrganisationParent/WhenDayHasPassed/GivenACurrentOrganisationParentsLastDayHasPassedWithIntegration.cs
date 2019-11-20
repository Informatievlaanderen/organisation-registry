namespace OrganisationRegistry.SqlServer.IntegrationTests.OnEventStore.OrganisationParent.WhenDayHasPassed
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
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;

    [Collection(EventStoreTestsCollection.Name)]
    public class GivenACurrentOrganisationParentsLastDayHasPassedWithIntegration : EventStoreIntegrationTestBase<DayHasPassed>
    {
        private OrganisationCreatedTestDataBuilder _organisationCreated;
        private OrganisationCreatedTestDataBuilder _parentOrganisationCreated;
        private static readonly DateTime DateTime = DateTime.Today;

        public GivenACurrentOrganisationParentsLastDayHasPassedWithIntegration(EventStoreSqlServerFixture fixture) :
            base(new DateTimeProviderStub(DateTime), fixture, new LoggerFactory())
        {
        }

        protected override IEnumerable<IEvent> Given()
        {
            var sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
            _organisationCreated = new OrganisationCreatedTestDataBuilder(sequentialOvoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedTestDataBuilder(sequentialOvoNumberGenerator);

            var organisationParentAdded = new OrganisationParentAddedTestDataBuilder(_organisationCreated.Id, _parentOrganisationCreated.Id)
                .WithValidity(DateTime, DateTime);

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
            ((DateTimeProviderStub) DateTimeProvider).StubDate = DateTime.AddDays(1);
            return new DayHasPassed(Guid.NewGuid(), DateTime);
        }

        [Fact]
        public async Task OrganisationParentIsCleared()
        {
            var waitAndRetry = Policy.Handle<AssertionFailedException>().RetryAsync(5);

            await waitAndRetry.ExecuteAsync(async () =>
            {
                await Task.Delay(1000);

                var organisationDetail =
                    AutofacServiceProvider.GetService<OrganisationRegistryContext>()
                        .OrganisationDetail
                        .Single(item => item.Id == _organisationCreated.Id);

                organisationDetail.ParentOrganisationId.Should().BeNull();
                organisationDetail.ParentOrganisation.Should().BeNullOrEmpty();

                var organisationListItem =
                    AutofacServiceProvider.GetService<OrganisationRegistryContext>()
                        .OrganisationList
                        .Where(item => item.OrganisationId == _organisationCreated.Id)
                        .ToList();

                organisationListItem.ForEach(item => item.ParentOrganisationId.Should().BeNull());
                organisationListItem.ForEach(item => item.ParentOrganisation.Should().BeNullOrEmpty());
            });
        }
    }
}
