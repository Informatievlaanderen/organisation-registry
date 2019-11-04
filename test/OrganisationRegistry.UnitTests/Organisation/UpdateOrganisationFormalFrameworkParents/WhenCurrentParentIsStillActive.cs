namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFrameworkParents
{
    using System;
    using System.Collections.Generic;
    using FormalFramework;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using Xunit.Abstractions;

    public class WhenCurrentParentIsStillActive: Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationFormalFrameworkParents>
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

            var organisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedTestDataBuilder(
                        _organisationCreated.Id,
                        _formalFrameworkCreated.Id,
                        _parentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today, _dateTimeProvider.Today.AddDays(1));

            var formalFrameworkAssignedToOrganisation =
                new FormalFrameworkAssignedToOrganisationTestDataBuilder(
                    organisationFormalFrameworkAdded.OrganisationFormalFrameworkId,
                    organisationFormalFrameworkAdded.FormalFrameworkId,
                    organisationFormalFrameworkAdded.OrganisationId,
                    organisationFormalFrameworkAdded.ParentOrganisationId);

            return new List<IEvent>
            {
                _organisationCreated.Build(),
                _parentOrganisationCreated.Build(),
                formalFrameworkCategoryCreated.Build(),
                _formalFrameworkCreated.Build(),
                organisationFormalFrameworkAdded.Build(),
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

        protected override int ExpectedNumberOfEvents => 0;

        public WhenCurrentParentIsStillActive(ITestOutputHelper helper) : base(helper) { }
    }
}
