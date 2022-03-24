namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFrameworkParents
{
    using System;
    using System.Collections.Generic;
    using Configuration;
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
    using Xunit.Abstractions;

    public class WhenCurrentParentIsStillActive: Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationFormalFrameworkParents>
    {
        private SequentialOvoNumberGenerator _sequentialOvoNumberGenerator;
        private DateTimeProviderStub _dateTimeProvider;
        private OrganisationCreatedBuilder _organisationCreated;
        private FormalFrameworkCreatedBuilder _formalFrameworkCreated;
        private OrganisationCreatedBuilder _parentOrganisationCreated;

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProvider = new DateTimeProviderStub(DateTime.Now);
            _sequentialOvoNumberGenerator =
                new SequentialOvoNumberGenerator();

            _organisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            _parentOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedBuilder();
            _formalFrameworkCreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);

            var organisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedBuilder(
                        _organisationCreated.Id,
                        _formalFrameworkCreated.Id,
                        _parentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today, _dateTimeProvider.Today.AddDays(1));

            var formalFrameworkAssignedToOrganisation =
                new FormalFrameworkAssignedToOrganisationBuilder(
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
                _dateTimeProvider, Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
        }

        protected override int ExpectedNumberOfEvents => 0;

        public WhenCurrentParentIsStillActive(ITestOutputHelper helper) : base(helper) { }
    }
}
