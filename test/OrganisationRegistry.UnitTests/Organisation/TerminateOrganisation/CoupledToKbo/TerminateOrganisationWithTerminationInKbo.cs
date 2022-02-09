namespace OrganisationRegistry.UnitTests.Organisation.TerminateOrganisation.CoupledToKbo
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Tests.Shared;
    using Tests.Shared.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    public class
        TerminateOrganisationWithTerminationInKbo : Specification<Organisation, OrganisationCommandHandlers,
            TerminateOrganisation>
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
                Kbo = new KboConfigurationStub
                {
                    KboV2LegalFormOrganisationClassificationTypeId = Guid.NewGuid(),
                    KboV2RegisteredOfficeLocationTypeId = Guid.NewGuid(),
                    KboV2FormalNameLabelTypeId = Guid.NewGuid(),
                }
            };
            _dateOfTermination = _dateTimeProviderStub.Today.AddDays(fixture.Create<int>());
            _organisationId = new OrganisationId(Guid.NewGuid());

            return new List<IEvent>
            {
                new OrganisationCreated(
                    _organisationId,
                    "organisation X",
                    "OVO001234",
                    "org", Article.None, "", new List<Purpose>(), false, new ValidFrom(), new ValidTo(),
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
                new OrganisationTerminationFoundInKbo(
                    _organisationId,
                    fixture.Create<string>(),
                    fixture.Create<DateTime>(),
                    fixture.Create<string>(),
                    fixture.Create<string>())
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


        protected override int ExpectedNumberOfEvents => 2;

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
            organisationTerminated.FieldsToTerminate.Functions.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.Labels.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.Locations.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.Relations.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.BankAccounts.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.FormalFrameworks.Should().BeEmpty();
            organisationTerminated.FieldsToTerminate.OpeningHours.Should().BeEmpty();

            organisationTerminated.ForcedKboTermination.Should().BeFalse();
            organisationTerminated.KboFieldsToTerminate.BankAccounts.Should().BeEmpty();
            organisationTerminated.KboFieldsToTerminate.FormalName.Should().BeNull();
            organisationTerminated.KboFieldsToTerminate.LegalForm.Should().BeNull();
            organisationTerminated.KboFieldsToTerminate.RegisteredOffice.Should().BeNull();
            organisationTerminated.DateOfTerminationAccordingToKbo.Should().NotBeNull();
        }

        [Fact]
        public void OrganisationTerminationSyncedWithKbo()
        {
            var organisationTerminationSyncedWithKbo =
                PublishedEvents[1].UnwrapBody<OrganisationTerminationSyncedWithKbo>();
            organisationTerminationSyncedWithKbo.Should().NotBeNull();

            organisationTerminationSyncedWithKbo.OrganisationBankAccountIdsToTerminate.Should().HaveCount(1);
            organisationTerminationSyncedWithKbo.FormalNameOrganisationLabelIdToTerminate.Should().NotBeNull();
            organisationTerminationSyncedWithKbo.RegisteredOfficeOrganisationLocationIdToTerminate.Should().BeNull();
            organisationTerminationSyncedWithKbo.LegalFormOrganisationOrganisationClassificationIdToTerminate.Should().BeNull();
        }

        public TerminateOrganisationWithTerminationInKbo(ITestOutputHelper helper) : base(helper)
        {
        }
    }
}
