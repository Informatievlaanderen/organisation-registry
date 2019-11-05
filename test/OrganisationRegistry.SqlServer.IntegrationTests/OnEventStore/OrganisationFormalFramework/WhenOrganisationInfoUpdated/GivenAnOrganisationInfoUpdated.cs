namespace OrganisationRegistry.SqlServer.IntegrationTests.OnEventStore.OrganisationFormalFramework.WhenOrganisationInfoUpdated
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using TestBases;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;

    [Collection(EventStoreTestsCollection.Name)]
    public class GivenAnOrganisationInfoUpdated : EventStoreIntegrationTestBase<OrganisationInfoUpdated>
    {
        private readonly IOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        private FormalFrameworkCreatedTestDataBuilder _formalFrameworkCreated;
        private OrganisationCreatedTestDataBuilder _childOrganisationCreated;
        private OrganisationCreatedTestDataBuilder _parentOrganisationCreated;
        private static readonly DateTime DateTime = DateTime.Today;

        public GivenAnOrganisationInfoUpdated(EventStoreSqlServerFixture fixture) :
            base(new DateTimeProviderStub(DateTime), fixture)
        {
        }

        protected override IEnumerable<IEvent> Given()
        {
            var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedTestDataBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
            _parentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var anotherParentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            _childOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);

            var organisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                    _childOrganisationCreated.Id,
                    _formalFrameworkCreated.Id,
                    _parentOrganisationCreated.Id);

            var anotherOrganisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                    _parentOrganisationCreated.Id,
                    _formalFrameworkCreated.Id,
                    _parentOrganisationCreated.Id);

            return new List<IEvent>
            {
                _parentOrganisationCreated.Build(),
                anotherParentOrganisationCreated.Build(),
                _childOrganisationCreated.Build(),
                formalFrameworkCategoryCreated.Build(),
                _formalFrameworkCreated.Build(),
                organisationFormalFrameworkAdded.Build(),
                anotherOrganisationFormalFrameworkAdded.Build(),
            };
        }

        protected override OrganisationInfoUpdated When()
        {
            ((DateTimeProviderStub) DateTimeProvider).StubDate = DateTime.AddDays(1);
            return new OrganisationInfoUpdatedTestDataBuilder(_parentOrganisationCreated.Build())
                .WithName("New name!")
                .Build();
        }

        [Fact]
        public void OrganisationParentIsCleared()
        {
            var organisationFormalFramework =
                AutofacServiceProvider.GetService<OrganisationRegistryContext>()
                    .OrganisationFormalFrameworkList
                    .Where(item => item.ParentOrganisationId == _parentOrganisationCreated.Id)
                    .ToList();

            organisationFormalFramework.ForEach(item => item.ParentOrganisationName.Should().Be("New name!"));
        }
    }
}
