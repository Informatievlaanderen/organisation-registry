namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFrameworkParents
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
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAnotherParentIsNowActive: Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationFormalFrameworkParents>
    {
        private SequentialOvoNumberGenerator _sequentialOvoNumberGenerator;
        private DateTimeProviderStub _dateTimeProvider;
        private OrganisationCreatedBuilder _organisationCreated;
        private OrganisationCreatedBuilder _parentOrganisationCreated;
        private OrganisationCreatedBuilder _newParentOrganisationCreated;
        private FormalFrameworkCreatedBuilder _formalFrameworkCreated;
        private FormalFrameworkCreatedBuilder _anotherFormalFrameworkCreated;

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProvider = new DateTimeProviderStub(DateTime.Now);
            _sequentialOvoNumberGenerator =
                new SequentialOvoNumberGenerator();

            _organisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            _newParentOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
            _anotherFormalFrameworkCreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);

            var organisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedBuilder(
                        _organisationCreated.Id,
                        _formalFrameworkCreated.Id,
                        _parentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1));

            var newOrganisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedBuilder(
                        _organisationCreated.Id,
                        _anotherFormalFrameworkCreated.Id,
                        _newParentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-1), _dateTimeProvider.Today.AddDays(0));

            var formalFrameworkAssignedToOrganisation = new FormalFrameworkAssignedToOrganisationBuilder(
                organisationFormalFrameworkAdded.OrganisationFormalFrameworkId,
                organisationFormalFrameworkAdded.FormalFrameworkId,
                organisationFormalFrameworkAdded.OrganisationId,
                organisationFormalFrameworkAdded.ParentOrganisationId);

            return new List<IEvent>
            {
                _organisationCreated.Build(),
                _parentOrganisationCreated.Build(),
                _newParentOrganisationCreated.Build(),
                formalFrameworkCategoryCreated.Build(),
                _formalFrameworkCreated.Build(),
                organisationFormalFrameworkAdded.Build(),
                newOrganisationFormalFrameworkAdded.Build(),
                formalFrameworkAssignedToOrganisation.Build()
            };
        }

        protected override UpdateOrganisationFormalFrameworkParents When()
        {
            return new UpdateOrganisationFormalFrameworkParents(
                _organisationCreated.Id,
                new FormalFrameworkId(_formalFrameworkCreated.Id));
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

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void PublishesFormalFrameworkClearedFromOrganisation()
        {
            var @event = PublishedEvents[0];
            @event.Should().BeOfType<Envelope<FormalFrameworkClearedFromOrganisation>>();
            var formalFrameworkClearedFromOrganisation = @event.UnwrapBody<FormalFrameworkClearedFromOrganisation>();
            formalFrameworkClearedFromOrganisation.OrganisationId.Should().Be((Guid)_organisationCreated.Id);
            formalFrameworkClearedFromOrganisation.FormalFrameworkId.Should().Be((Guid)_formalFrameworkCreated.Id);
            formalFrameworkClearedFromOrganisation.ParentOrganisationId.Should().Be((Guid)_parentOrganisationCreated.Id);
        }

        public WhenAnotherParentIsNowActive(ITestOutputHelper helper) : base(helper) { }
    }
}
