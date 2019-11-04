namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationFormalFramework
{
    using System;
    using System.Collections.Generic;
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

    public class WhenAddingAnOrganisationFormalFrameworkWithCircularDependenciesButNotInTheSameValidity : ExceptionSpecification<Organisation, OrganisationCommandHandlers, AddOrganisationFormalFramework>
    {
        private DateTimeProviderStub _dateTimeProviderStub;
        private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new SequentialOvoNumberGenerator();

        private OrganisationCreatedTestDataBuilder _childOrganisationCreated;
        private OrganisationCreatedTestDataBuilder _parentOrganisationCreated;
        private FormalFrameworkCreatedTestDataBuilder _formalFrameworkCreated;
        private FormalFrameworkCategoryCreatedTestDataBuilder _formalFrameworkCategoryCreated;
        private OrganisationFormalFrameworkAddedTestDataBuilder _childBecameDaughterOfParent;
        private DateTime _tomorrow;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                _ovoNumberGenerator,
                null,
                _dateTimeProviderStub,
                Mock.Of<IOrganisationRegistryConfiguration>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
            _tomorrow = _dateTimeProviderStub.Today.AddDays(1);

            _childOrganisationCreated = new OrganisationCreatedTestDataBuilder(_ovoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_ovoNumberGenerator);
            _formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedTestDataBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(_formalFrameworkCategoryCreated.Id, _formalFrameworkCategoryCreated.Name);
            _childBecameDaughterOfParent =
                new OrganisationFormalFrameworkAddedTestDataBuilder(_childOrganisationCreated.Id, _formalFrameworkCreated.Id, _parentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProviderStub.Today, _dateTimeProviderStub.Today);

            return new List<IEvent>
            {
                _childOrganisationCreated.Build(),
                _parentOrganisationCreated.Build(),
                _formalFrameworkCategoryCreated.Build(),
                _formalFrameworkCreated.Build(),
                _childBecameDaughterOfParent.Build()
            };
        }

        protected override AddOrganisationFormalFramework When()
        {
            return new AddOrganisationFormalFramework(
                Guid.NewGuid(),
                new FormalFrameworkId(_formalFrameworkCreated.Id),
                _parentOrganisationCreated.Id,
                _childOrganisationCreated.Id,
                new ValidFrom(_tomorrow), new ValidTo(_tomorrow));
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void DoesNotThrowAnException()
        {
            Exception.Should().BeNull();
        }

        [Fact]
        public void AnOrganisationParentWasAdded()
        {
            var organisationParentAdded = PublishedEvents[0].UnwrapBody<OrganisationFormalFrameworkAdded>();

            organisationParentAdded.OrganisationId.Should().Be(_parentOrganisationCreated.Id);
            organisationParentAdded.ParentOrganisationId.Should().Be(_childOrganisationCreated.Id);
            organisationParentAdded.ValidFrom.Should().Be(_tomorrow);
            organisationParentAdded.ValidTo.Should().Be(_tomorrow);
        }

        public WhenAddingAnOrganisationFormalFrameworkWithCircularDependenciesButNotInTheSameValidity(ITestOutputHelper helper) : base(helper) { }
    }
}
