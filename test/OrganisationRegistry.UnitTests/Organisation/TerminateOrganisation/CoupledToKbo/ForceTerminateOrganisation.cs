namespace OrganisationRegistry.UnitTests.Organisation.TerminateOrganisation.CoupledToKbo
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
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Tests.Shared;
    using Xunit;
    using Xunit.Abstractions;

    public class ForceTerminateOrganisation: Specification<Organisation, OrganisationCommandHandlers, TerminateOrganisation>
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
                    "org",
                    "",
                    new List<Purpose>(),
                    false,
                    new ValidFrom(),
                    new ValidTo()),
                new OrganisationCoupledWithKbo(
                    _organisationId,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    null),
                new KboOrganisationBankAccountAdded(
                    _organisationId,
                    fixture.Create<Guid>(),
                    fixture.Create<string>(),
                    fixture.Create<bool>(),
                    fixture.Create<string>(),
                    fixture.Create<bool>(),
                    null,
                    null),
                new KboFormalNameLabelAdded(
                    _organisationId,
                    fixture.Create<Guid>(),
                    fixture.Create<Guid>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    null,
                    null),
            };
        }

        protected override TerminateOrganisation When()
        {
            return new TerminateOrganisation(
                _organisationId,
                _dateOfTermination,
                true,
                new ClaimsPrincipal());
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

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void TerminatesTheOrganisation()
        {
            var organisationTerminated = PublishedEvents[0].UnwrapBody<OrganisationTerminated>();
            organisationTerminated.Should().NotBeNull();

            organisationTerminated.OrganisationId.Should().Be((Guid) _organisationId);
            organisationTerminated.OrganisationNewValidTo.Should().Be(_dateOfTermination);
            organisationTerminated.OvoNumber.Should().Be("OVO001234");
            organisationTerminated.BuildingsToTerminate.Should().BeEmpty();
            organisationTerminated.CapacitiesToTerminate.Should().BeEmpty();
            organisationTerminated.ClassificationsToTerminate.Should().BeEmpty();
            organisationTerminated.ContactsToTerminate.Should().BeEmpty();
            organisationTerminated.DateOfTermination.Should().Be(_dateOfTermination);
            organisationTerminated.FunctionsToTerminate.Should().BeEmpty();
            organisationTerminated.LabelsToTerminate.Should().BeEmpty();
            organisationTerminated.LocationsToTerminate.Should().BeEmpty();
            organisationTerminated.ParentsToTerminate.Should().BeEmpty();
            organisationTerminated.RelationsToTerminate.Should().BeEmpty();
            organisationTerminated.BankAccountsToTerminate.Should().BeEmpty();
            organisationTerminated.FormalFrameworksToTerminate.Should().BeEmpty();
            organisationTerminated.OpeningHoursToTerminate.Should().BeEmpty();

            organisationTerminated.ForcedKboTermination.Should().BeTrue();
            organisationTerminated.KboBankAccountsToTerminate.Should().HaveCount(1);
            organisationTerminated.KboFormalNameToTerminate.Should().NotBeNull();
            organisationTerminated.KboLegalFormToTerminate.Should().BeNull();
            organisationTerminated.KboRegisteredOfficeToTerminate.Should().BeNull();
            organisationTerminated.DateOfTerminationAccordingToKbo.Should().BeNull();
        }

        public ForceTerminateOrganisation(ITestOutputHelper helper) : base(helper) { }
    }
}
