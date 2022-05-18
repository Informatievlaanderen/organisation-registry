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
    using OrganisationRegistry.Organisation.Exceptions;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAddingAnOrganisationFormalFrameworkWithOverlappingValidity : ExceptionSpecification<
        AddOrganisationFormalFrameworkCommandHandler, AddOrganisationFormalFramework>
    {
        private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
        private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new();

        private OrganisationCreatedBuilder _childOrganisationCreated;
        private OrganisationCreatedBuilder _parentOrganisationCreated;
        private FormalFrameworkCreatedBuilder _formalFrameworkCreated;
        private FormalFrameworkCategoryCreatedBuilder _formalFrameworkCategoryCreated;
        private OrganisationFormalFrameworkAddedBuilder _childBecameDaughterOfParent;

        public WhenAddingAnOrganisationFormalFrameworkWithOverlappingValidity(ITestOutputHelper helper) : base(helper)
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
            _childOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
            _formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedBuilder(
                _formalFrameworkCategoryCreated.Id,
                _formalFrameworkCategoryCreated.Name);
            _childBecameDaughterOfParent = new OrganisationFormalFrameworkAddedBuilder(
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
                _childOrganisationCreated.Id,
                new OrganisationId(_parentOrganisationCreated.Id),
                new ValidFrom(),
                new ValidTo());

        protected override int ExpectedNumberOfEvents
            => 0;

        [Fact]
        public void ThrowsADomainException()
        {
            Exception.Should().BeOfType<OrganisationAlreadyCoupledToFormalFrameworkParentInThisPeriod>();
        }
    }
}
