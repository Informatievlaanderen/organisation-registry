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

    public class WhenCurrentParentIsNoLongerActive: Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationFormalFrameworkParents>
    {
        private SequentialOvoNumberGenerator _sequentialOvoNumberGenerator;
        private DateTimeProviderStub _dateTimeProvider;
        private OrganisationCreatedTestDataBuilder _organisationCreated;
        private FormalFrameworkCreatedTestDataBuilder _formalFrameworkCreated;
        private OrganisationCreatedTestDataBuilder _parentOrganisationCreated;

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProvider = new DateTimeProviderStub(DateTime.Now);
            _sequentialOvoNumberGenerator =
                new SequentialOvoNumberGenerator();

            _organisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedTestDataBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
            var anotherFormalFrameworkCreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);

            var organisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                        _organisationCreated.Id,
                        _formalFrameworkCreated.Id,
                        _parentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1));

            var anotherOrganisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                        _organisationCreated.Id,
                        anotherFormalFrameworkCreated.Id,
                        _parentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1));

            var formalFrameworkAssignedToOrganisation = new FormalFrameworkAssignedToOrganisationTestDataBuilder(
                organisationFormalFrameworkAdded.OrganisationFormalFrameworkId,
                organisationFormalFrameworkAdded.FormalFrameworkId,
                organisationFormalFrameworkAdded.OrganisationId,
                organisationFormalFrameworkAdded.ParentOrganisationId);

            var anotherFormalFrameworkAssignedToOrganisation = new FormalFrameworkAssignedToOrganisationTestDataBuilder(
                anotherOrganisationFormalFrameworkAdded.OrganisationFormalFrameworkId,
                anotherOrganisationFormalFrameworkAdded.FormalFrameworkId,
                anotherOrganisationFormalFrameworkAdded.OrganisationId,
                anotherOrganisationFormalFrameworkAdded.ParentOrganisationId);

            return new List<IEvent>
            {
                _organisationCreated.Build(),
                _parentOrganisationCreated.Build(),
                formalFrameworkCategoryCreated.Build(),
                _formalFrameworkCreated.Build(),
                anotherFormalFrameworkCreated.Build(),
                organisationFormalFrameworkAdded.Build(),
                anotherOrganisationFormalFrameworkAdded.Build(),
                formalFrameworkAssignedToOrganisation.Build(),
                anotherFormalFrameworkAssignedToOrganisation.Build()
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
            PublishedEvents[0].Should().BeOfType<Envelope<FormalFrameworkClearedFromOrganisation>>();
            var formalFrameworkClearedFromOrganisation = PublishedEvents[0].UnwrapBody<FormalFrameworkClearedFromOrganisation>();
            formalFrameworkClearedFromOrganisation.OrganisationId.Should().Be((Guid)_organisationCreated.Id);
            formalFrameworkClearedFromOrganisation.FormalFrameworkId.Should().Be((Guid)_formalFrameworkCreated.Id);
            formalFrameworkClearedFromOrganisation.ParentOrganisationId.Should().Be((Guid)_parentOrganisationCreated.Id);
        }

        public WhenCurrentParentIsNoLongerActive(ITestOutputHelper helper) : base(helper) { }
    }
}
