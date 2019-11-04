namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFrameworkParents
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenOtherParentsAreNowActive: Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationFormalFrameworkParents>
    {
        private SequentialOvoNumberGenerator _sequentialOvoNumberGenerator;
        private DateTimeProviderStub _dateTimeProvider;
        private OrganisationCreatedTestDataBuilder _childOrganisationCreated;
        private OrganisationCreatedTestDataBuilder _parentOrganisationCreated;
        private OrganisationCreatedTestDataBuilder _anotherParentOrganisationCreated;
        private FormalFrameworkCreatedTestDataBuilder _formalFrameworkCreated;
        private FormalFrameworkCreatedTestDataBuilder _anotherFormalFrameworkCreated;

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProvider = new DateTimeProviderStub(DateTime.Now);
            _sequentialOvoNumberGenerator =
                new SequentialOvoNumberGenerator();

            _childOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            _anotherParentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedTestDataBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
            _anotherFormalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);

            var organisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                        _childOrganisationCreated.Id,
                        _formalFrameworkCreated.Id,
                        _parentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1));

            var newOrganisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                        _childOrganisationCreated.Id,
                        _formalFrameworkCreated.Id,
                        _anotherParentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-1), _dateTimeProvider.Today.AddDays(0));

            var organisationFormalFrameworkBAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                        _childOrganisationCreated.Id,
                        _anotherFormalFrameworkCreated.Id,
                        _parentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-1), _dateTimeProvider.Today.AddDays(0));

            var anotherOrganisationFormalFrameworkBAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                        _childOrganisationCreated.Id,
                        _anotherFormalFrameworkCreated.Id,
                        _anotherParentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1));

            var formalFrameworkAssignedToOrganisation = new FormalFrameworkAssignedToOrganisationTestDataBuilder(
                organisationFormalFrameworkAdded.OrganisationFormalFrameworkId,
                organisationFormalFrameworkAdded.FormalFrameworkId,
                organisationFormalFrameworkAdded.OrganisationId,
                organisationFormalFrameworkAdded.ParentOrganisationId);

            var anotherFormalFrameworkAssignedToOrganisation = new FormalFrameworkAssignedToOrganisationTestDataBuilder(
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
                _dateTimeProvider, Mock.Of<IOrganisationRegistryConfiguration>());
        }

        protected override int ExpectedNumberOfEvents => 2;

        [Fact]
        public void PublishesFormalFrameworkClearedFromOrganisation()
        {
            var @event = PublishedEvents[0];
            @event.Should().BeOfType<Envelope<FormalFrameworkClearedFromOrganisation>>();
            var frameworkClearedFromOrganisation = @event.UnwrapBody<FormalFrameworkClearedFromOrganisation>();
            frameworkClearedFromOrganisation.OrganisationId.Should().Be(_childOrganisationCreated.Id);
            frameworkClearedFromOrganisation.ParentOrganisationId.Should().Be(_parentOrganisationCreated.Id);
            frameworkClearedFromOrganisation.FormalFrameworkId.Should().Be(_formalFrameworkCreated.Id);
        }

        [Fact]
        public void PublishesAnotherFormalFrameworkAssignedToOrganisation()
        {
            var @event = PublishedEvents[1];
            @event.Should().BeOfType<Envelope<FormalFrameworkAssignedToOrganisation>>();
            var formalFrameworkAssignedToOrganisation = @event.UnwrapBody<FormalFrameworkAssignedToOrganisation>();
            formalFrameworkAssignedToOrganisation.OrganisationId.Should().Be(_childOrganisationCreated.Id);
            formalFrameworkAssignedToOrganisation.ParentOrganisationId.Should().Be(_anotherParentOrganisationCreated.Id);
            formalFrameworkAssignedToOrganisation.FormalFrameworkId.Should().Be(_formalFrameworkCreated.Id);
        }

        public WhenOtherParentsAreNowActive(ITestOutputHelper helper) : base(helper) { }
    }
}
