namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFrameworkParents
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using FormalFramework;
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

    public class WhenAnotherParentIsNowActive: Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationFormalFrameworkParents>
    {
        private SequentialOvoNumberGenerator _sequentialOvoNumberGenerator;
        private DateTimeProviderStub _dateTimeProvider;
        private OrganisationCreatedTestDataBuilder _organisationCreated;
        private OrganisationCreatedTestDataBuilder _parentOrganisationCreated;
        private OrganisationCreatedTestDataBuilder _newParentOrganisationCreated;
        private FormalFrameworkCreatedTestDataBuilder _formalFrameworkCreated;
        private FormalFrameworkCreatedTestDataBuilder _anotherFormalFrameworkCreated;

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProvider = new DateTimeProviderStub(DateTime.Now);
            _sequentialOvoNumberGenerator =
                new SequentialOvoNumberGenerator();

            _organisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            _newParentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedTestDataBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
            _anotherFormalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);

            var organisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                        _organisationCreated.Id,
                        _formalFrameworkCreated.Id,
                        _parentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1));

            var newOrganisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                        _organisationCreated.Id,
                        _anotherFormalFrameworkCreated.Id,
                        _newParentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-1), _dateTimeProvider.Today.AddDays(0));

            var formalFrameworkAssignedToOrganisation = new FormalFrameworkAssignedToOrganisationTestDataBuilder(
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
                _dateTimeProvider, Mock.Of<IOrganisationRegistryConfiguration>());
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
