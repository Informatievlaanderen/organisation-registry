namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationFormalFramework
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using FormalFramework;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAddingAnOrganisationFormalFramework : Specification<Organisation, OrganisationCommandHandlers, AddOrganisationFormalFramework>
    {
        private DateTimeProviderStub _dateTimeProviderStub;
        private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new SequentialOvoNumberGenerator();

        private OrganisationCreatedTestDataBuilder _childOrganisationCreated;
        private OrganisationCreatedTestDataBuilder _parentOrganisationCreated;
        private FormalFrameworkCreatedTestDataBuilder _formalFrameworkCreated;
        private FormalFrameworkCategoryCreatedTestDataBuilder _formalFrameworkCategoryCreatedTestDataBuilder;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                _ovoNumberGenerator,
                null,
                _dateTimeProviderStub,
                Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

            _childOrganisationCreated = new OrganisationCreatedTestDataBuilder(_ovoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_ovoNumberGenerator);
            _formalFrameworkCategoryCreatedTestDataBuilder = new FormalFrameworkCategoryCreatedTestDataBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(_formalFrameworkCategoryCreatedTestDataBuilder.Id, _formalFrameworkCategoryCreatedTestDataBuilder.Name);

            return new List<IEvent>
            {
                _childOrganisationCreated.Build(),
                _parentOrganisationCreated.Build(),
                _formalFrameworkCategoryCreatedTestDataBuilder.Build(),
                _formalFrameworkCreated.Build()
            };
        }

        protected override AddOrganisationFormalFramework When()
        {
            return new AddOrganisationFormalFramework(
                Guid.NewGuid(),
                new FormalFrameworkId(_formalFrameworkCreated.Id),
                _childOrganisationCreated.Id,
                _parentOrganisationCreated.Id,
                new ValidFrom(), new ValidTo());
        }

        protected override int ExpectedNumberOfEvents => 2;

        [Fact]
        public void AddsAnOrganisationParent()
        {
            var organisationParentAdded = PublishedEvents[0].UnwrapBody<OrganisationFormalFrameworkAdded>();

            organisationParentAdded.OrganisationFormalFrameworkId.Should().NotBeEmpty();
            organisationParentAdded.FormalFrameworkId.Should().Be((Guid)_formalFrameworkCreated.Id);
            organisationParentAdded.OrganisationId.Should().Be((Guid)_childOrganisationCreated.Id);
            organisationParentAdded.ParentOrganisationId.Should().Be((Guid)_parentOrganisationCreated.Id);
            organisationParentAdded.ValidFrom.Should().BeNull();
            organisationParentAdded.ValidTo.Should().BeNull();
        }

        [Fact]
        public void AssignsAParent()
        {
            var parentAssignedToOrganisation = PublishedEvents[1].UnwrapBody<FormalFrameworkAssignedToOrganisation>();
            parentAssignedToOrganisation.OrganisationId.Should().Be((Guid)_childOrganisationCreated.Id);
            parentAssignedToOrganisation.FormalFrameworkId.Should().Be((Guid)_formalFrameworkCreated.Id);
            parentAssignedToOrganisation.ParentOrganisationId.Should().Be((Guid)_parentOrganisationCreated.Id);
        }

        public WhenAddingAnOrganisationFormalFramework(ITestOutputHelper helper) : base(helper) { }
    }
}
