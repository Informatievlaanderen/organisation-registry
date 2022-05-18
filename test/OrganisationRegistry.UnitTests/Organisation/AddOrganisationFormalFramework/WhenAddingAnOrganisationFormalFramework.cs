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
    using OrganisationRegistry.Infrastructure.Configuration;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAddingAnOrganisationFormalFramework : Specification<AddOrganisationFormalFrameworkCommandHandler,
        AddOrganisationFormalFramework>
    {
        private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
        private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new();

        private OrganisationCreatedBuilder _childOrganisationCreated;
        private OrganisationCreatedBuilder _parentOrganisationCreated;
        private FormalFrameworkCreatedBuilder _formalFrameworkCreated;
        private FormalFrameworkCategoryCreatedBuilder _formalFrameworkCategoryCreatedBuilder;

        public WhenAddingAnOrganisationFormalFramework(ITestOutputHelper helper) : base(helper)
        {
        }

        protected override AddOrganisationFormalFrameworkCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<AddOrganisationFormalFrameworkCommandHandler>>().Object,
                Session,
                _dateTimeProviderStub,
                Mock.Of<IOrganisationRegistryConfiguration>()
            );

        protected override IUser User
            => new UserBuilder()
                .AddRoles(Role.AlgemeenBeheerder)
                .Build();

        protected override IEnumerable<IEvent> Given()
        {
            _childOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
            _formalFrameworkCategoryCreatedBuilder = new FormalFrameworkCategoryCreatedBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedBuilder(
                _formalFrameworkCategoryCreatedBuilder.Id,
                _formalFrameworkCategoryCreatedBuilder.Name);

            return new List<IEvent>
            {
                _childOrganisationCreated.Build(),
                _parentOrganisationCreated.Build(),
                _formalFrameworkCategoryCreatedBuilder.Build(),
                _formalFrameworkCreated.Build()
            };
        }

        protected override AddOrganisationFormalFramework When()
            => new AddOrganisationFormalFramework(
                Guid.NewGuid(),
                new FormalFrameworkId(_formalFrameworkCreated.Id),
                _childOrganisationCreated.Id,
                _parentOrganisationCreated.Id,
                new ValidFrom(),
                new ValidTo());

        protected override int ExpectedNumberOfEvents
            => 2;

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
    }
}
