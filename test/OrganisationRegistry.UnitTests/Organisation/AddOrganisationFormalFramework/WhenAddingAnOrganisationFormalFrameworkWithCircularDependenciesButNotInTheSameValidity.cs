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

    public class WhenAddingAnOrganisationFormalFrameworkWithCircularDependenciesButNotInTheSameValidity :
        ExceptionSpecification<AddOrganisationFormalFrameworkCommandHandler, AddOrganisationFormalFramework>
    {
        private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
        private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new();

        private OrganisationCreatedBuilder _childOrganisationCreated;
        private OrganisationCreatedBuilder _parentOrganisationCreated;
        private FormalFrameworkCreatedBuilder _formalFrameworkCreated;
        private FormalFrameworkCategoryCreatedBuilder _formalFrameworkCategoryCreated;
        private OrganisationFormalFrameworkAddedBuilder _childBecameDaughterOfParent;
        private DateTime _tomorrow;

        public WhenAddingAnOrganisationFormalFrameworkWithCircularDependenciesButNotInTheSameValidity(
            ITestOutputHelper helper) : base(helper)
        {
        }

        protected override AddOrganisationFormalFrameworkCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<AddOrganisationFormalFrameworkCommandHandler>>().Object,
                Session,

                _dateTimeProviderStub,
                Mock.Of<IOrganisationRegistryConfiguration>());

        protected override IUser User
            => new UserBuilder()
                .AddRoles(Role.AlgemeenBeheerder)
                .Build();

        protected override IEnumerable<IEvent> Given()
        {
            _tomorrow = _dateTimeProviderStub.Today.AddDays(1);

            _childOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
            _formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedBuilder(
                _formalFrameworkCategoryCreated.Id,
                _formalFrameworkCategoryCreated.Name);
            _childBecameDaughterOfParent =
                new OrganisationFormalFrameworkAddedBuilder(
                        _childOrganisationCreated.Id,
                        _formalFrameworkCreated.Id,
                        _parentOrganisationCreated.Id)
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
            => new(
                Guid.NewGuid(),
                new FormalFrameworkId(_formalFrameworkCreated.Id),
                _parentOrganisationCreated.Id,
                _childOrganisationCreated.Id,
                new ValidFrom(_tomorrow),
                new ValidTo(_tomorrow));

        protected override int ExpectedNumberOfEvents
            => 1;

        [Fact]
        public void DoesNotThrowAnException()
        {
            Exception.Should().BeNull();
        }

        [Fact]
        public void AnOrganisationParentWasAdded()
        {
            var organisationParentAdded = PublishedEvents[0].UnwrapBody<OrganisationFormalFrameworkAdded>();

            organisationParentAdded.OrganisationId.Should().Be((Guid)_parentOrganisationCreated.Id);
            organisationParentAdded.ParentOrganisationId.Should().Be((Guid)_childOrganisationCreated.Id);
            organisationParentAdded.ValidFrom.Should().Be(_tomorrow);
            organisationParentAdded.ValidTo.Should().Be(_tomorrow);
        }
    }
}
