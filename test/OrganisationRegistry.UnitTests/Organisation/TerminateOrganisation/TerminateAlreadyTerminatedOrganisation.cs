namespace OrganisationRegistry.UnitTests.Organisation.TerminateOrganisation
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using AutoFixture;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Kbo;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Organisation.OrganisationTermination;
    using OrganisationRegistry.Organisation.State;
    using Tests.Shared;
    using Xunit;
    using Xunit.Abstractions;

    public class TerminateAlreadyTerminatedOrganisation: ExceptionSpecification<Organisation, OrganisationCommandHandlers, TerminateOrganisation>
    {
        private OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub;

        private OrganisationId _organisationId;
        private DateTimeProviderStub _dateTimeProviderStub;
        private DateTime _dateOfTermination;

        protected override IEnumerable<IEvent> Given()
        {
            var fixture = new Fixture();
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Today);
            _organisationRegistryConfigurationStub = new OrganisationRegistryConfigurationStub
            {
                FormalFrameworkIdsToTerminateEndOfNextYear = new []{Guid.NewGuid()},
                OrganisationCapacityTypeIdsToTerminateEndOfNextYear = new []{Guid.NewGuid()},
                OrganisationClassificationTypeIdsToTerminateEndOfNextYear = new []{Guid.NewGuid()}
            };
            _dateOfTermination = _dateTimeProviderStub.Today.AddDays(fixture.Create<int>());
            _organisationId = new OrganisationId(Guid.NewGuid());

            return new List<IEvent>
            {
                new OrganisationCreated(
                    _organisationId,
                    "organisation X",
                    "OVO001234",
                    "org", Article.None, "", new List<Purpose>(), false, new ValidFrom(), new ValidTo()),
                OrganisationTerminated.Create(
                    _organisationId,
                    new OrganisationState(),
                    new KboState(),
                    new OrganisationTerminationSummary(),
                    false,
                    new OrganisationTerminationKboSummary(),
                    fixture.Create<DateTime>())
            };
        }

        protected override TerminateOrganisation When()
        {
            return new TerminateOrganisation(
                    _organisationId,
                    _dateOfTermination,
                    false)
                .WithUserRole(Role.OrganisationRegistryBeheerder);
        }

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                _dateTimeProviderStub,
                _organisationRegistryConfigurationStub);
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsOrganisationAlreadyCoupledWithKbo()
        {
            Exception.Should().BeOfType<OrganisationAlreadyTerminated>();
        }

        public TerminateAlreadyTerminatedOrganisation(ITestOutputHelper helper) : base(helper) { }
    }
}
