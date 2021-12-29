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
    using OrganisationRegistry.Infrastructure.Authorization;
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
                    "org", Article.None, "", new List<Purpose>(), false, new ValidFrom(), new ValidTo(), new ValidFrom(), new ValidTo()),
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
                _organisationRegistryConfigurationStub,
                Mock.Of<ISecurityService>());
        }


        protected override int ExpectedNumberOfEvents => 3;

        [Fact]
        public void TerminatesTheOrganisation()
        {
            var organisationTerminated = PublishedEvents[0].UnwrapBody<OrganisationTerminatedV2>();
            organisationTerminated.Should().NotBeNull();

            organisationTerminated.OrganisationId.Should().Be((Guid) _organisationId);
            organisationTerminated.FieldsToTerminate.OrganisationValidity.Should().Be(_dateOfTermination);
            organisationTerminated.OvoNumber.Should().Be("OVO001234");
            organisationTerminated.FieldsToTerminate.Buildings.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.Capacities.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.Classifications.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.Contacts.Should().BeEmpty();
            organisationTerminated.DateOfTermination.Should().Be(_dateOfTermination);
            organisationTerminated.ForcedKboTermination.Should().BeFalse();
            organisationTerminated.FieldsToTerminate.Functions.Should().BeEmpty();
            organisationTerminated.KboFieldsToTerminate.FormalName.Should().BeNull();
            organisationTerminated.KboFieldsToTerminate.LegalForm.Should().BeNull();
            organisationTerminated.KboFieldsToTerminate.RegisteredOffice.Should().BeNull();
            organisationTerminated.FieldsToTerminate.Labels.Should().HaveCount(2);
            organisationTerminated.FieldsToTerminate.Locations.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.Relations.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.BankAccounts.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.FormalFrameworks.Should().HaveCount(1);
            organisationTerminated.FieldsToTerminate.OpeningHours.Should().BeEmpty();
            organisationTerminated.KboFieldsToTerminate.BankAccounts.Should().BeEmpty();
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
        public void FormalFrameworkClearedFromOrganisation()
        {
            var frameworkClearedFromOrganisation = PublishedEvents[2].UnwrapBody<FormalFrameworkClearedFromOrganisation>();
            frameworkClearedFromOrganisation.Should().NotBeNull();

            frameworkClearedFromOrganisation.OrganisationId.Should().Be((Guid) _organisationId);
        }

        public TerminateOrganisationThatBecomesInvalid(ITestOutputHelper helper) : base(helper) { }
    }
}
