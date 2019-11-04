namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using FormalFramework;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenUpdatingAnOrganisationFormalFramework : Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationFormalFramework>
    {
        private static readonly DateTimeProviderStub DateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new SequentialOvoNumberGenerator();

        private OrganisationCreatedTestDataBuilder _childOrganisationCreated;
        private OrganisationCreatedTestDataBuilder _parentOrganisationACreated;
        private OrganisationCreatedTestDataBuilder _parentOrganisationBCreated;
        private FormalFrameworkCreatedTestDataBuilder _formalFrameworkACreated;
        private FormalFrameworkCreatedTestDataBuilder _formalFrameworkBCreated;
        private FormalFrameworkCategoryCreatedTestDataBuilder _formalFrameworkCategoryCreatedTestDataBuilder;
        private OrganisationFormalFrameworkAddedTestDataBuilder _childBecameDaughterOfParent;
        private readonly DateTime? _tomorrow = DateTimeProviderStub.Today.AddDays(1);

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                new DateTimeProvider(),
                Mock.Of<IOrganisationRegistryConfiguration>());
        }

        protected override IEnumerable<IEvent> Given()
        {

            _childOrganisationCreated = new OrganisationCreatedTestDataBuilder(_ovoNumberGenerator);
            _parentOrganisationACreated = new OrganisationCreatedTestDataBuilder(_ovoNumberGenerator);
            _parentOrganisationBCreated = new OrganisationCreatedTestDataBuilder(_ovoNumberGenerator);
            _formalFrameworkCategoryCreatedTestDataBuilder = new FormalFrameworkCategoryCreatedTestDataBuilder();
            _formalFrameworkACreated = new FormalFrameworkCreatedTestDataBuilder(_formalFrameworkCategoryCreatedTestDataBuilder.Id, _formalFrameworkCategoryCreatedTestDataBuilder.Name);
            _formalFrameworkBCreated = new FormalFrameworkCreatedTestDataBuilder(_formalFrameworkCategoryCreatedTestDataBuilder.Id, _formalFrameworkCategoryCreatedTestDataBuilder.Name);
            _childBecameDaughterOfParent =
                new OrganisationFormalFrameworkAddedTestDataBuilder(_childOrganisationCreated.Id, _formalFrameworkACreated.Id, _parentOrganisationACreated.Id);

            return new List<IEvent>
            {
                _childOrganisationCreated.Build(),
                _parentOrganisationACreated.Build(),
                _parentOrganisationBCreated.Build(),
                _formalFrameworkCategoryCreatedTestDataBuilder.Build(),
                _formalFrameworkACreated.Build(),
                _formalFrameworkBCreated.Build(),
                _childBecameDaughterOfParent.Build()
            };
        }

        protected override UpdateOrganisationFormalFramework When()
        {
            return new UpdateOrganisationFormalFramework(
                _childBecameDaughterOfParent.OrganisationFormalFrameworkId,
                new FormalFrameworkId(_formalFrameworkBCreated.Id),
                _childOrganisationCreated.Id,
                _parentOrganisationBCreated.Id,
                new ValidFrom(_tomorrow), new ValidTo(_tomorrow));
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void UpdatesTheOrganisationBuilding()
        {
            PublishedEvents[0].Should().BeOfType<Envelope<OrganisationFormalFrameworkUpdated>>();

            var organisationFormalFrameworkUpdated = PublishedEvents.First().UnwrapBody<OrganisationFormalFrameworkUpdated>();
            organisationFormalFrameworkUpdated.OrganisationId.Should().Be(_childOrganisationCreated.Id);
            organisationFormalFrameworkUpdated.PreviousParentOrganisationId.Should().Be(_parentOrganisationACreated.Id);
            organisationFormalFrameworkUpdated.ParentOrganisationId.Should().Be(_parentOrganisationBCreated.Id);
            organisationFormalFrameworkUpdated.FormalFrameworkId.Should().Be(_formalFrameworkBCreated.Id);
            organisationFormalFrameworkUpdated.ValidFrom.Should().Be(_tomorrow);
            organisationFormalFrameworkUpdated.ValidTo.Should().Be(_tomorrow);
        }

        public WhenUpdatingAnOrganisationFormalFramework(ITestOutputHelper helper) : base(helper) { }
    }
}
