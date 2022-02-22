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
    using OrganisationRegistry.Infrastructure.Authorization;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Tests.Shared.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenUpdatingAnOrganisationFormalFrameworkValidityToTheFuture : Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationFormalFramework>
    {
        private static readonly DateTimeProviderStub DateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new SequentialOvoNumberGenerator();

        private OrganisationCreatedTestDataBuilder _childOrganisationCreated;
        private OrganisationCreatedTestDataBuilder _parentOrganisationACreated;
        private OrganisationCreatedTestDataBuilder _parentOrganisationBCreated;
        private FormalFrameworkCreatedTestDataBuilder _formalFrameworkACreated;
        private FormalFrameworkCategoryCreatedTestDataBuilder _formalFrameworkCategoryCreatedTestDataBuilder;
        private OrganisationFormalFrameworkAddedTestDataBuilder _childBecameDaughterOfParent;
        private readonly DateTime? _tomorrow = DateTimeProviderStub.Today.AddDays(1);
        private FormalFrameworkAssignedToOrganisationTestDataBuilder _formalFrameworkAssignedToOrg;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                new DateTimeProvider(),
                new OrganisationRegistryConfigurationStub(),
                Mock.Of<ISecurityService>());
        }

        protected override IEnumerable<IEvent> Given()
        {

            _childOrganisationCreated = new OrganisationCreatedTestDataBuilder(_ovoNumberGenerator);
            _parentOrganisationACreated = new OrganisationCreatedTestDataBuilder(_ovoNumberGenerator);
            _parentOrganisationBCreated = new OrganisationCreatedTestDataBuilder(_ovoNumberGenerator);
            _formalFrameworkCategoryCreatedTestDataBuilder = new FormalFrameworkCategoryCreatedTestDataBuilder();
            _formalFrameworkACreated = new FormalFrameworkCreatedTestDataBuilder(_formalFrameworkCategoryCreatedTestDataBuilder.Id, _formalFrameworkCategoryCreatedTestDataBuilder.Name);
            _childBecameDaughterOfParent = new OrganisationFormalFrameworkAddedTestDataBuilder(_childOrganisationCreated.Id, _formalFrameworkACreated.Id, _parentOrganisationACreated.Id);
            _formalFrameworkAssignedToOrg =
                new FormalFrameworkAssignedToOrganisationTestDataBuilder(
                    _childBecameDaughterOfParent.OrganisationFormalFrameworkId, _childBecameDaughterOfParent.FormalFrameworkId, _childOrganisationCreated.Id, _parentOrganisationACreated.Id);

            return new List<IEvent>
            {
                _childOrganisationCreated.Build(),
                _parentOrganisationACreated.Build(),
                _parentOrganisationBCreated.Build(),
                _formalFrameworkCategoryCreatedTestDataBuilder.Build(),
                _formalFrameworkACreated.Build(),
                _childBecameDaughterOfParent.Build(),
                _formalFrameworkAssignedToOrg.Build()
            };
        }

        protected override UpdateOrganisationFormalFramework When()
        {
            return new UpdateOrganisationFormalFramework(
                _childBecameDaughterOfParent.OrganisationFormalFrameworkId,
                new FormalFrameworkId(_formalFrameworkACreated.Id),
                _childOrganisationCreated.Id,
                _parentOrganisationBCreated.Id,
                new ValidFrom(_tomorrow), new ValidTo(_tomorrow))
            {
                User = new UserBuilder()
                    .AddRoles(Role.OrganisationRegistryBeheerder)
                    .Build()
            };
        }

        protected override int ExpectedNumberOfEvents => 2;

        [Fact]
        public void UpdatesTheOrganisationFormalFramework()
        {
            PublishedEvents[0].Should().BeOfType<Envelope<OrganisationFormalFrameworkUpdated>>();

            var organisationFormalFrameworkUpdated = PublishedEvents.First().UnwrapBody<OrganisationFormalFrameworkUpdated>();
            organisationFormalFrameworkUpdated.OrganisationId.Should().Be((Guid)_childOrganisationCreated.Id);
            organisationFormalFrameworkUpdated.ParentOrganisationId.Should().Be((Guid)_parentOrganisationBCreated.Id);
            organisationFormalFrameworkUpdated.ValidFrom.Should().Be(_tomorrow);
            organisationFormalFrameworkUpdated.ValidTo.Should().Be(_tomorrow);
        }

        [Fact]
        public void ClearsAParent()
        {
            var formalFrameworkClearedFromOrganisation = PublishedEvents[1].UnwrapBody<FormalFrameworkClearedFromOrganisation>();
            formalFrameworkClearedFromOrganisation.OrganisationId.Should().Be((Guid)_childOrganisationCreated.Id);
            formalFrameworkClearedFromOrganisation.ParentOrganisationId.Should().Be((Guid)_parentOrganisationBCreated.Id);
            formalFrameworkClearedFromOrganisation.FormalFrameworkId.Should().Be((Guid)_formalFrameworkACreated.Id);
        }

        public WhenUpdatingAnOrganisationFormalFrameworkValidityToTheFuture(ITestOutputHelper helper) : base(helper) { }
    }
}
