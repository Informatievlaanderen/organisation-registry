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
    using OrganisationRegistry.Organisation.Exceptions;
    using Tests.Shared.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenUpdatingAnOrganisationFormalFrameworkWithCircularDependencies : ExceptionSpecification<
        UpdateOrganisationFormalFrameworkCommandHandler, UpdateOrganisationFormalFramework>
    {
        private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new();

        private OrganisationCreatedBuilder _childOrganisationCreated;
        private OrganisationCreatedBuilder _parentOrganisationCreated;
        private OrganisationCreatedBuilder _grandParentOrganisationCreated;
        private FormalFrameworkCreatedBuilder _formalFrameworkCreated;
        private FormalFrameworkCategoryCreatedBuilder _formalFrameworkCategoryCreatedBuilder;
        private OrganisationFormalFrameworkAddedBuilder _childBecameDaughterOfParent;
        private OrganisationFormalFrameworkAddedBuilder _parentBecameDaughterOfGrandParent;
        private OrganisationCreatedBuilder _greatGrandParentOrganisationCreated;
        private OrganisationFormalFrameworkAddedBuilder _grandParentBecameDaughterOfGrandParent;

        public WhenUpdatingAnOrganisationFormalFrameworkWithCircularDependencies(ITestOutputHelper helper) : base(
            helper)
        {
        }

        protected override UpdateOrganisationFormalFrameworkCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<UpdateOrganisationFormalFrameworkCommandHandler>>().Object,
                Session,
                new DateTimeProvider(),
                new OrganisationRegistryConfigurationStub());

        protected override IUser User
            => new UserBuilder()
                .AddRoles(Role.AlgemeenBeheerder)
                .Build();

        protected override IEnumerable<IEvent> Given()
        {
            _childOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
            _grandParentOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
            _greatGrandParentOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
            _formalFrameworkCategoryCreatedBuilder = new FormalFrameworkCategoryCreatedBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedBuilder(
                _formalFrameworkCategoryCreatedBuilder.Id,
                _formalFrameworkCategoryCreatedBuilder.Name);
            _childBecameDaughterOfParent =
                new OrganisationFormalFrameworkAddedBuilder(
                    _childOrganisationCreated.Id,
                    _formalFrameworkCreated.Id,
                    _parentOrganisationCreated.Id);
            _parentBecameDaughterOfGrandParent =
                new OrganisationFormalFrameworkAddedBuilder(
                    _parentOrganisationCreated.Id,
                    _formalFrameworkCreated.Id,
                    _grandParentOrganisationCreated.Id);
            _grandParentBecameDaughterOfGrandParent =
                new OrganisationFormalFrameworkAddedBuilder(
                    _grandParentOrganisationCreated.Id,
                    _formalFrameworkCreated.Id,
                    _greatGrandParentOrganisationCreated.Id);

            return new List<IEvent>
            {
                _childOrganisationCreated.Build(),
                _parentOrganisationCreated.Build(),
                _grandParentOrganisationCreated.Build(),
                _greatGrandParentOrganisationCreated.Build(),
                _formalFrameworkCategoryCreatedBuilder.Build(),
                _formalFrameworkCreated.Build(),
                _childBecameDaughterOfParent.Build(),
                _parentBecameDaughterOfGrandParent.Build(),
                _grandParentBecameDaughterOfGrandParent.Build()
            };
        }

        protected override UpdateOrganisationFormalFramework When()
            => new(
                _grandParentBecameDaughterOfGrandParent.OrganisationFormalFrameworkId,
                new FormalFrameworkId(_formalFrameworkCreated.Id),
                _grandParentOrganisationCreated.Id,
                _childOrganisationCreated.Id,
                new ValidFrom(),
                new ValidTo());

        protected override int ExpectedNumberOfEvents
            => 0;

        [Fact]
        public void ThrowsADomainException()
        {
            Exception.Should().BeOfType<CircularRelationInFormalFramework>();
        }
    }
}
