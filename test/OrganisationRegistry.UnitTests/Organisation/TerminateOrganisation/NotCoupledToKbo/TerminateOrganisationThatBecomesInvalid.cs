namespace OrganisationRegistry.UnitTests.Organisation.TerminateOrganisation.NotCoupledToKbo
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

    public class TerminateOrganisationThatBecomesInvalid: Specification<Organisation, OrganisationCommandHandlers, TerminateOrganisation>
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
            _dateOfTermination = _dateTimeProviderStub.Today.AddDays(fixture.Create<int>() * -1);
            _organisationId = new OrganisationId(Guid.NewGuid());

            var organisationOrganisationParentId = fixture.Create<Guid>();
            var parentOrganisationId = fixture.Create<Guid>();
            var organisationFormalFrameworkId = fixture.Create<Guid>();
            var formalFrameworkId = fixture.Create<Guid>();
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
                new OrganisationLabelAdded(
                    _organisationId,
                    fixture.Create<Guid>(),
                    fixture.Create<Guid>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    _dateOfTermination.AddDays(fixture.Create<int>()*-1),
                    _dateOfTermination.AddDays(fixture.Create<int>())),
                new OrganisationLabelAdded(
                    _organisationId,
                    fixture.Create<Guid>(),
                    fixture.Create<Guid>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    _dateOfTermination.AddDays(fixture.Create<int>()*-1),
                    _dateOfTermination.AddDays(fixture.Create<int>())),
                new OrganisationBecameActive(_organisationId),
                new OrganisationParentAdded(
                    _organisationId,
                    organisationOrganisationParentId,
                    parentOrganisationId,
                    fixture.Create<string>(),
                    null,
                    null),
                new ParentAssignedToOrganisation(
                    _organisationId,
                    parentOrganisationId,
                    organisationOrganisationParentId),
                new OrganisationFormalFrameworkAdded(
                    _organisationId,
                    organisationFormalFrameworkId,
                    formalFrameworkId,
                    fixture.Create<string>(),
                    parentOrganisationId,
                    fixture.Create<string>(),
                    null,
                    null),
                new FormalFrameworkAssignedToOrganisation(
                    _organisationId,
                    formalFrameworkId,
                    parentOrganisationId,
                    organisationFormalFrameworkId)
            };
        }

        protected override TerminateOrganisation When()
        {
            return new TerminateOrganisation(
                _organisationId,
                _dateOfTermination,
                false,
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

        protected override int ExpectedNumberOfEvents => 4;

        [Fact]
        public void TerminatesTheOrganisation()
        {
            var organisationTerminated = PublishedEvents[0].UnwrapBody<OrganisationTerminated>();
            organisationTerminated.Should().NotBeNull();

            organisationTerminated.OrganisationId.Should().Be((Guid) _organisationId);
            organisationTerminated.FieldsToTerminate.OrganisationNewValidTo.Should().Be(_dateOfTermination);
            organisationTerminated.OvoNumber.Should().Be("OVO001234");
            organisationTerminated.FieldsToTerminate.BuildingsToTerminate.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.CapacitiesToTerminate.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.ClassificationsToTerminate.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.ContactsToTerminate.Should().BeEmpty();
            organisationTerminated.DateOfTermination.Should().Be(_dateOfTermination);
            organisationTerminated.ForcedKboTermination.Should().BeFalse();
            organisationTerminated.FieldsToTerminate.FunctionsToTerminate.Should().BeEmpty();
            organisationTerminated.KboFieldsToTerminate.KboFormalNameToTerminate.Should().BeNull();
            organisationTerminated.KboFieldsToTerminate.KboLegalFormToTerminate.Should().BeNull();
            organisationTerminated.KboFieldsToTerminate.KboRegisteredOfficeToTerminate.Should().BeNull();
            organisationTerminated.FieldsToTerminate.LabelsToTerminate.Should().HaveCount(2);
            organisationTerminated.FieldsToTerminate.LocationsToTerminate.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.ParentsToTerminate.Should().HaveCount(1);
            organisationTerminated.FieldsToTerminate.RelationsToTerminate.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.BankAccountsToTerminate.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.FormalFrameworksToTerminate.Should().HaveCount(1);
            organisationTerminated.FieldsToTerminate.OpeningHoursToTerminate.Should().BeEmpty();
            organisationTerminated.KboFieldsToTerminate.KboBankAccountsToTerminate.Should().BeEmpty();
            organisationTerminated.DateOfTerminationAccordingToKbo.Should().BeNull();
        }

        [Fact]
        public void OrganisationBecomesInactive()
        {
            var organisationBecameInactive = PublishedEvents[1].UnwrapBody<OrganisationBecameInactive>();
            organisationBecameInactive.Should().NotBeNull();

            organisationBecameInactive.OrganisationId.Should().Be((Guid) _organisationId);
        }

        [Fact]
        public void ParentClearedFromOrganisation()
        {
            var parentClearedFromOrganisation = PublishedEvents[2].UnwrapBody<ParentClearedFromOrganisation>();
            parentClearedFromOrganisation.Should().NotBeNull();

            parentClearedFromOrganisation.OrganisationId.Should().Be((Guid) _organisationId);
        }

        [Fact]
        public void FormalFrameworkClearedFromOrganisation()
        {
            var frameworkClearedFromOrganisation = PublishedEvents[3].UnwrapBody<FormalFrameworkClearedFromOrganisation>();
            frameworkClearedFromOrganisation.Should().NotBeNull();

            frameworkClearedFromOrganisation.OrganisationId.Should().Be((Guid) _organisationId);
        }

        public TerminateOrganisationThatBecomesInvalid(ITestOutputHelper helper) : base(helper) { }
    }
}
