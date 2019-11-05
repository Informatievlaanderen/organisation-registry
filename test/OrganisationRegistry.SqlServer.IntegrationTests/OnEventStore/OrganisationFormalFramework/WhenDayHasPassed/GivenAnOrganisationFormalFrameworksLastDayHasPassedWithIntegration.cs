namespace OrganisationRegistry.SqlServer.IntegrationTests.OnEventStore.OrganisationFormalFramework.WhenDayHasPassed
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
    using Polly;
    using TestBases;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using Xunit;

    [Collection(EventStoreTestsCollection.Name)]
    public class GivenAnOrganisationFormalFrameworksLastDayHasPassedWithIntegration : EventStoreIntegrationTestBase<DayHasPassed>
    {
        private readonly IOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        private FormalFrameworkCreatedTestDataBuilder _formalFrameworkCreated;
        private OrganisationCreatedTestDataBuilder _childOrganisationCreated;
        private static readonly DateTime DateTime = DateTime.Today;

        public GivenAnOrganisationFormalFrameworksLastDayHasPassedWithIntegration(EventStoreSqlServerFixture fixture) :
            base(new DateTimeProviderStub(DateTime), fixture)
        {
        }

        protected override IEnumerable<IEvent> Given()
        {
            var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedTestDataBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
            var parentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var anotherFormalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
            _childOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);

            var organisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                        _childOrganisationCreated.Id,
                        _formalFrameworkCreated.Id,
                        parentOrganisationCreated.Id)
                    .WithValidity(DateTime, DateTime);

            var anotherOrganisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                        _childOrganisationCreated.Id,
                        anotherFormalFrameworkCreated.Id,
                        parentOrganisationCreated.Id)
                    .WithValidity(DateTime, DateTime);

            var formalFrameworkAssigned =
                new FormalFrameworkAssignedToOrganisationTestDataBuilder(
                    organisationFormalFrameworkAdded.OrganisationFormalFrameworkId,
                    organisationFormalFrameworkAdded.FormalFrameworkId,
                    organisationFormalFrameworkAdded.OrganisationId,
                    organisationFormalFrameworkAdded.ParentOrganisationId);

            var anotherFormalFrameworkAssigned =
                new FormalFrameworkAssignedToOrganisationTestDataBuilder(
                    anotherOrganisationFormalFrameworkAdded.OrganisationFormalFrameworkId,
                    anotherOrganisationFormalFrameworkAdded.FormalFrameworkId,
                    anotherOrganisationFormalFrameworkAdded.OrganisationId,
                    anotherOrganisationFormalFrameworkAdded.ParentOrganisationId);

            return new List<IEvent>
            {
                parentOrganisationCreated.Build(),
                _childOrganisationCreated.Build(),
                formalFrameworkCategoryCreated.Build(),
                _formalFrameworkCreated.Build(),
                anotherFormalFrameworkCreated.Build(),
                organisationFormalFrameworkAdded.Build(),
                anotherOrganisationFormalFrameworkAdded.Build(),
                formalFrameworkAssigned.Build(),
                anotherFormalFrameworkAssigned.Build()
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
                var organisationDetailItem =
                    AutofacServiceProvider.GetService<OrganisationRegistryContext>()
                        .OrganisationDetail
                        .Single(item => item.Id == _childOrganisationCreated.Id);

                organisationDetailItem.FormalFrameworkId.Should().BeNull();

                var organisationListItems =
                    AutofacServiceProvider.GetService<OrganisationRegistryContext>()
                        .OrganisationList
                        .Where(item =>
                            item.OrganisationId == _childOrganisationCreated.Id &&
                            item.FormalFrameworkId != null)
                        .ToList();

                organisationListItems.ForEach(item => item.ParentOrganisationId.Should().BeNull());
                organisationListItems.ForEach(item => item.ParentOrganisation.Should().BeNull());
            });
        }
    }
}
