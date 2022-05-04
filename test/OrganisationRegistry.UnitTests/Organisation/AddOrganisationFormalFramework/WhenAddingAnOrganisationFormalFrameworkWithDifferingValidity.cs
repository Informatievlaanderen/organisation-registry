namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationFormalFramework
{
    using System;
    using System.Collections.Generic;
    using Configuration;
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
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAddingAnOrganisationFormalFrameworkWithDifferingValidity : OldSpecification<Organisation, OrganisationCommandHandlers, AddOrganisationFormalFramework>
    {
        private DateTimeProviderStub _dateTimeProviderStub;
        private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new SequentialOvoNumberGenerator();

        private OrganisationCreatedBuilder _childOrganisationCreated;
        private OrganisationCreatedBuilder _parentOrganisationCreated;
        private FormalFrameworkCreatedBuilder _formalFrameworkCreated;
        private FormalFrameworkCategoryCreatedBuilder _formalFrameworkCategoryCreated;
        private OrganisationFormalFrameworkAddedBuilder _childBecameDaughterOfParent;
        private DateTime _tomorrow;

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
            _tomorrow = _dateTimeProviderStub.Today.AddDays(1);

            _childOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
            _formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedBuilder(_formalFrameworkCategoryCreated.Id, _formalFrameworkCategoryCreated.Name);
            _childBecameDaughterOfParent =
                new OrganisationFormalFrameworkAddedBuilder(_childOrganisationCreated.Id,_formalFrameworkCreated.Id, _parentOrganisationCreated.Id)
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
                _childOrganisationCreated.Id,
                _parentOrganisationCreated.Id,
                new ValidFrom(_tomorrow), new ValidTo(_tomorrow))
            {
                User = new UserBuilder()
                    .AddRoles(Role.AlgemeenBeheerder)
                    .Build()
            };
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void AddsAnOrganisationParent()
        {
            var organisationParentAdded = PublishedEvents[0].UnwrapBody<OrganisationFormalFrameworkAdded>();

            organisationParentAdded.FormalFrameworkId.Should().Be((Guid)_formalFrameworkCreated.Id);
            organisationParentAdded.OrganisationId.Should().Be((Guid)_childOrganisationCreated.Id);
            organisationParentAdded.ParentOrganisationId.Should().Be((Guid)_parentOrganisationCreated.Id);
            organisationParentAdded.ValidFrom.Should().Be(_tomorrow);
            organisationParentAdded.ValidTo.Should().Be(_tomorrow);
        }

        public WhenAddingAnOrganisationFormalFrameworkWithDifferingValidity(ITestOutputHelper helper) : base(helper) { }
    }
}
