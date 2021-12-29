namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFramework
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
    using Xunit;
    using Xunit.Abstractions;

    public class WhenUpdatingAnOrganisationFormalFrameworkWithCircularDependencies : ExceptionSpecification<Organisation, OrganisationCommandHandlers, UpdateOrganisationFormalFramework>
    {
        private static readonly DateTimeProviderStub DateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new SequentialOvoNumberGenerator();

        private OrganisationCreatedTestDataBuilder _childOrganisationCreated;
        private OrganisationCreatedTestDataBuilder _parentOrganisationCreated;
        private OrganisationCreatedTestDataBuilder _grandParentOrganisationCreated;
        private FormalFrameworkCreatedTestDataBuilder _formalFrameworkCreated;
        private FormalFrameworkCategoryCreatedTestDataBuilder _formalFrameworkCategoryCreatedTestDataBuilder;
        private OrganisationFormalFrameworkAddedTestDataBuilder _childBecameDaughterOfParent;
        private OrganisationFormalFrameworkAddedTestDataBuilder _parentBecameDaughterOfGrandParent;
        private OrganisationCreatedTestDataBuilder _greatGrandParentOrganisationCreated;
        private OrganisationFormalFrameworkAddedTestDataBuilder _grandParentBecameDaughterOfGrandParent;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                new DateTimeProvider(),
                Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
        }

        protected override IEnumerable<IEvent> Given()
        {

            _childOrganisationCreated = new OrganisationCreatedTestDataBuilder(_ovoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_ovoNumberGenerator);
            _grandParentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_ovoNumberGenerator);
            _greatGrandParentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_ovoNumberGenerator);
            _formalFrameworkCategoryCreatedTestDataBuilder = new FormalFrameworkCategoryCreatedTestDataBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(_formalFrameworkCategoryCreatedTestDataBuilder.Id, _formalFrameworkCategoryCreatedTestDataBuilder.Name);
            _childBecameDaughterOfParent =
                new OrganisationFormalFrameworkAddedTestDataBuilder(_childOrganisationCreated.Id, _formalFrameworkCreated.Id, _parentOrganisationCreated.Id);
            _parentBecameDaughterOfGrandParent =
                new OrganisationFormalFrameworkAddedTestDataBuilder(_parentOrganisationCreated.Id, _formalFrameworkCreated.Id, _grandParentOrganisationCreated.Id);
            _grandParentBecameDaughterOfGrandParent =
                new OrganisationFormalFrameworkAddedTestDataBuilder(_grandParentOrganisationCreated.Id, _formalFrameworkCreated.Id, _greatGrandParentOrganisationCreated.Id);

            return new List<IEvent>
            {
                _childOrganisationCreated.Build(),
                _parentOrganisationCreated.Build(),
                _grandParentOrganisationCreated.Build(),
                _greatGrandParentOrganisationCreated.Build(),
                _formalFrameworkCategoryCreatedTestDataBuilder.Build(),
                _formalFrameworkCreated.Build(),
                _childBecameDaughterOfParent.Build(),
                _parentBecameDaughterOfGrandParent.Build(),
                _grandParentBecameDaughterOfGrandParent.Build()
            };
        }

        protected override UpdateOrganisationFormalFramework When()
        {
            return new UpdateOrganisationFormalFramework(
                _grandParentBecameDaughterOfGrandParent.OrganisationFormalFrameworkId,
                new FormalFrameworkId(_formalFrameworkCreated.Id),
                _grandParentOrganisationCreated.Id,
                _childOrganisationCreated.Id,
                new ValidFrom(), new ValidTo());
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsADomainException()
        {
            Exception.Should().BeOfType<CircularRelationInFormalFrameworkException>();
        }

        public WhenUpdatingAnOrganisationFormalFrameworkWithCircularDependencies(ITestOutputHelper helper) : base(helper) { }
    }
}
