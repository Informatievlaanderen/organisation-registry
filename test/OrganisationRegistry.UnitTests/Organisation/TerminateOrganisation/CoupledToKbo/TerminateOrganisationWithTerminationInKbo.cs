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
    using OrganisationRegistry.Organisation.OrganisationTermination;
    using Tests.Shared.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    public class
        TerminateOrganisationWithTerminationInKbo : Specification<TerminateOrganisationCommandHandler, TerminateOrganisation>
    {
        private readonly OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub = new()
        {
            Kbo = new KboConfigurationStub
            {
                KboV2LegalFormOrganisationClassificationTypeId = Guid.NewGuid(),
                KboV2RegisteredOfficeLocationTypeId = Guid.NewGuid(),
                KboV2FormalNameLabelTypeId = Guid.NewGuid(),
            }
        };

        private readonly OrganisationId _organisationId = new(Guid.NewGuid());
        private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Today);
        private DateTime _dateOfTermination;

        public TerminateOrganisationWithTerminationInKbo(ITestOutputHelper helper) : base(helper)
        {
        }

        protected override IUser User
            => new UserBuilder()
                .AddRoles(Role.AlgemeenBeheerder)
                .Build();

        protected override IEnumerable<IEvent> Given()
        {
            var fixture = new Fixture();

            _dateOfTermination = _dateTimeProviderStub.Today.AddDays(fixture.Create<int>());

            return new List<IEvent>
            {
                new OrganisationCreated(
                    _organisationId,
                    "organisation X",
                    "OVO001234",
                    "org",
                    Article.None,
                    "",
                    new List<Purpose>(),
                    false,
                    new ValidFrom(),
                    new ValidTo(),
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
            => new(_organisationId, _dateOfTermination, false);

        protected override TerminateOrganisationCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<TerminateOrganisationCommandHandler>>().Object,
                Session,
                _dateTimeProviderStub,
                _organisationRegistryConfigurationStub);

        protected override int ExpectedNumberOfEvents
            => 2;

        [Fact]
        public void TerminatesTheOrganisation()
        {
            var organisationTerminated = PublishedEvents[0].UnwrapBody<OrganisationTerminatedV2>();
            organisationTerminated.Should().NotBeNull();

            organisationTerminated.OrganisationId.Should().Be((Guid)_organisationId);
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
    }
}
