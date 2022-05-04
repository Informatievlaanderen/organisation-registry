namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFrameworkParents
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using FluentAssertions;
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

    public class WhenOtherParentsAreNowActive: OldSpecification<Organisation, OrganisationCommandHandlers, UpdateOrganisationFormalFrameworkParents>
    {
        private SequentialOvoNumberGenerator _sequentialOvoNumberGenerator;
        private DateTimeProviderStub _dateTimeProvider;
        private OrganisationCreatedBuilder _childOrganisationCreated;
        private OrganisationCreatedBuilder _parentOrganisationCreated;
        private OrganisationCreatedBuilder _anotherParentOrganisationCreated;
        private FormalFrameworkCreatedBuilder _formalFrameworkCreated;
        private FormalFrameworkCreatedBuilder _anotherFormalFrameworkCreated;

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProvider = new DateTimeProviderStub(DateTime.Now);
            _sequentialOvoNumberGenerator =
                new SequentialOvoNumberGenerator();

            _childOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            _anotherParentOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
            _anotherFormalFrameworkCreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);

            var organisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedBuilder(
                        _childOrganisationCreated.Id,
                        _formalFrameworkCreated.Id,
                        _parentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1));

            var newOrganisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedBuilder(
                        _childOrganisationCreated.Id,
                        _formalFrameworkCreated.Id,
                        _anotherParentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-1), _dateTimeProvider.Today.AddDays(0));

            var organisationFormalFrameworkBAdded =
                new OrganisationFormalFrameworkAddedBuilder(
                        _childOrganisationCreated.Id,
                        _anotherFormalFrameworkCreated.Id,
                        _parentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-1), _dateTimeProvider.Today.AddDays(0));

            var anotherOrganisationFormalFrameworkBAdded =
                new OrganisationFormalFrameworkAddedBuilder(
                        _childOrganisationCreated.Id,
                        _anotherFormalFrameworkCreated.Id,
                        _anotherParentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1));

            var formalFrameworkAssignedToOrganisation = new FormalFrameworkAssignedToOrganisationBuilder(
                organisationFormalFrameworkAdded.OrganisationFormalFrameworkId,
                organisationFormalFrameworkAdded.FormalFrameworkId,
                organisationFormalFrameworkAdded.OrganisationId,
                organisationFormalFrameworkAdded.ParentOrganisationId);

            var anotherFormalFrameworkAssignedToOrganisation = new FormalFrameworkAssignedToOrganisationBuilder(
                anotherOrganisationFormalFrameworkBAdded.OrganisationFormalFrameworkId,
                anotherOrganisationFormalFrameworkBAdded.FormalFrameworkId,
                anotherOrganisationFormalFrameworkBAdded.OrganisationId,
                anotherOrganisationFormalFrameworkBAdded.ParentOrganisationId);

            return new List<IEvent>
            {
                _childOrganisationCreated.Build(),
                _parentOrganisationCreated.Build(),
                _anotherParentOrganisationCreated.Build(),
                formalFrameworkCategoryCreated.Build(),
                _formalFrameworkCreated.Build(),
                _anotherFormalFrameworkCreated.Build(),
                organisationFormalFrameworkBAdded.Build(),
                anotherOrganisationFormalFrameworkBAdded.Build(),
                organisationFormalFrameworkAdded.Build(),
                newOrganisationFormalFrameworkAdded.Build(),
                formalFrameworkAssignedToOrganisation.Build(),
                anotherFormalFrameworkAssignedToOrganisation.Build()
            };
        }

        protected override UpdateOrganisationFormalFrameworkParents When()
        {
            return new UpdateOrganisationFormalFrameworkParents(_childOrganisationCreated.Id, _formalFrameworkCreated.Id);
        }

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                Mock.Of<ILogger<OrganisationCommandHandlers>>(),
                Session,
                _sequentialOvoNumberGenerator,
                new UniqueOvoNumberValidatorStub(true),
                _dateTimeProvider, Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
        }

        protected override int ExpectedNumberOfEvents => 2;

        [Fact]
        public void PublishesFormalFrameworkClearedFromOrganisation()
        {
            var @event = PublishedEvents[0];
            @event.Should().BeOfType<Envelope<FormalFrameworkClearedFromOrganisation>>();
            var frameworkClearedFromOrganisation = @event.UnwrapBody<FormalFrameworkClearedFromOrganisation>();
            frameworkClearedFromOrganisation.OrganisationId.Should().Be((Guid)_childOrganisationCreated.Id);
            frameworkClearedFromOrganisation.ParentOrganisationId.Should().Be((Guid)_parentOrganisationCreated.Id);
            frameworkClearedFromOrganisation.FormalFrameworkId.Should().Be((Guid)_formalFrameworkCreated.Id);
        }

        [Fact]
        public void PublishesAnotherFormalFrameworkAssignedToOrganisation()
        {
            var @event = PublishedEvents[1];
            @event.Should().BeOfType<Envelope<FormalFrameworkAssignedToOrganisation>>();
            var formalFrameworkAssignedToOrganisation = @event.UnwrapBody<FormalFrameworkAssignedToOrganisation>();
            formalFrameworkAssignedToOrganisation.OrganisationId.Should().Be((Guid)_childOrganisationCreated.Id);
            formalFrameworkAssignedToOrganisation.ParentOrganisationId.Should().Be((Guid)_anotherParentOrganisationCreated.Id);
            formalFrameworkAssignedToOrganisation.FormalFrameworkId.Should().Be((Guid)_formalFrameworkCreated.Id);
        }

        public WhenOtherParentsAreNowActive(ITestOutputHelper helper) : base(helper) { }
    }
}
