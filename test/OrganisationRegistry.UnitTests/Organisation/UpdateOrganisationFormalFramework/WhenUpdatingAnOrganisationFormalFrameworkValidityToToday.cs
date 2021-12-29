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
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenUpdatingAnOrganisationFormalFrameworkValidityToToday : Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationFormalFramework>
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
                Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
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
                new OrganisationFormalFrameworkAddedTestDataBuilder(_childOrganisationCreated.Id, _formalFrameworkACreated.Id, _parentOrganisationACreated.Id)
                    .WithValidity(_tomorrow, _tomorrow);

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
                new ValidFrom(DateTimeProviderStub.Today), new ValidTo(DateTimeProviderStub.Today));
        }

        protected override int ExpectedNumberOfEvents => 2;

        [Fact]
        public void UpdatesTheOrganisationFormalFramework()
        {
            PublishedEvents[0].Should().BeOfType<Envelope<OrganisationFormalFrameworkUpdated>>();

            var organisationFormalFrameworkUpdated = PublishedEvents[0].UnwrapBody<OrganisationFormalFrameworkUpdated>();
            organisationFormalFrameworkUpdated.OrganisationId.Should().Be((Guid)_childOrganisationCreated.Id);
            organisationFormalFrameworkUpdated.ParentOrganisationId.Should().Be((Guid)_parentOrganisationBCreated.Id);
            organisationFormalFrameworkUpdated.ValidFrom.Should().Be(DateTimeProviderStub.Today);
            organisationFormalFrameworkUpdated.ValidTo.Should().Be(DateTimeProviderStub.Today);
        }

        [Fact]
        public void AssignsAParent()
        {
            var frameworkAssignedToOrganisation = PublishedEvents[1].UnwrapBody<FormalFrameworkAssignedToOrganisation>();
            frameworkAssignedToOrganisation.OrganisationId.Should().Be((Guid)_childOrganisationCreated.Id);
            frameworkAssignedToOrganisation.ParentOrganisationId.Should().Be((Guid)_parentOrganisationBCreated.Id);
            frameworkAssignedToOrganisation.FormalFrameworkId.Should().Be((Guid)_formalFrameworkBCreated.Id);
        }

        public WhenUpdatingAnOrganisationFormalFrameworkValidityToToday(ITestOutputHelper helper) : base(helper) { }
    }
}
