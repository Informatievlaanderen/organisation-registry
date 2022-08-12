namespace OrganisationRegistry.UnitTests.Organisation.TerminateOrganisation.NotCoupledToKbo;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class
    TerminateOrganisationNotCoupledToKbo : Specification<TerminateOrganisationCommandHandler, TerminateOrganisation>
{
    private readonly OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub;

    private readonly Guid _organisationId;
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly DateTime _dateOfTermination;
    private readonly Fixture _fixture;

    public TerminateOrganisationNotCoupledToKbo(ITestOutputHelper helper) : base(helper)
    {
        _organisationRegistryConfigurationStub = new OrganisationRegistryConfigurationStub
        {
            Kbo = new KboConfigurationStub
            {
                KboV2LegalFormOrganisationClassificationTypeId = Guid.NewGuid(),
                KboV2RegisteredOfficeLocationTypeId = Guid.NewGuid(),
                KboV2FormalNameLabelTypeId = Guid.NewGuid(),
            },
        };
        _organisationId = Guid.NewGuid();
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Today);
        _fixture = new Fixture();

        _dateOfTermination = _dateTimeProviderStub.Today.AddDays(_fixture.Create<int>());
    }


    private IEvent[] Events
        => new IEvent[]
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
            new OrganisationLabelAdded(
                _organisationId,
                _fixture.Create<Guid>(),
                _fixture.Create<Guid>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _dateOfTermination.AddDays(_fixture.Create<int>() * -1),
                _dateOfTermination.AddDays(_fixture.Create<int>())
            ),
            new OrganisationLabelAdded(
                _organisationId,
                _fixture.Create<Guid>(),
                _fixture.Create<Guid>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _dateOfTermination.AddDays(_fixture.Create<int>() * -1),
                _dateOfTermination.AddDays(_fixture.Create<int>())
            ),
        };

    private TerminateOrganisation TerminateOrganisationCommand
        => new(new OrganisationId(_organisationId), _dateOfTermination, false);

    protected override TerminateOrganisationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<TerminateOrganisationCommandHandler>>().Object,
            session,
            _dateTimeProviderStub,
            _organisationRegistryConfigurationStub);

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events).When(TerminateOrganisationCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Theory]
    [InlineData(Role.AlgemeenBeheerder)]
    [InlineData(Role.CjmBeheerder)]
    public async Task TerminatesTheOrganisation(Role role)
    {
        await Given(Events).When(TerminateOrganisationCommand, new UserBuilder().AddRoles(role).Build()).Then();

        var organisationTerminated = PublishedEvents[0].UnwrapBody<OrganisationTerminatedV2>();
        organisationTerminated.Should().NotBeNull();

        organisationTerminated.OrganisationId.Should().Be(_organisationId);
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
        organisationTerminated.FieldsToTerminate.FormalFrameworks.Should().BeEmpty();
        organisationTerminated.FieldsToTerminate.OpeningHours.Should().BeEmpty();
        organisationTerminated.KboFieldsToTerminate.BankAccounts.Should().BeEmpty();
        organisationTerminated.DateOfTerminationAccordingToKbo.Should().BeNull();
    }
}
