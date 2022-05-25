namespace OrganisationRegistry.UnitTests.Organisation.TerminateOrganisation.NotCoupledToKbo;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class
    TerminateOrganisationThatBecomesInvalid : Specification<TerminateOrganisationCommandHandler, TerminateOrganisation>
{
    private readonly OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub;

    private readonly Guid _organisationId;
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly DateTime _dateOfTermination;
    private readonly Guid _organisationOrganisationParentId;
    private readonly Guid _parentOrganisationId;
    private readonly Guid _organisationFormalFrameworkId;
    private readonly Guid _formalFrameworkId;
    private readonly Fixture _fixture;

    public TerminateOrganisationThatBecomesInvalid(ITestOutputHelper helper) : base(helper)
    {
        _organisationRegistryConfigurationStub = new OrganisationRegistryConfigurationStub
        {
            Kbo = new KboConfigurationStub
            {
                KboV2LegalFormOrganisationClassificationTypeId = Guid.NewGuid(),
                KboV2RegisteredOfficeLocationTypeId = Guid.NewGuid(),
                KboV2FormalNameLabelTypeId = Guid.NewGuid(),
            }
        };
        _organisationId = Guid.NewGuid();
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Today);
        _fixture = new Fixture();

        _dateOfTermination = _dateTimeProviderStub.Today.AddDays(_fixture.Create<int>());
        _organisationOrganisationParentId = _fixture.Create<Guid>();
        _parentOrganisationId = _fixture.Create<Guid>();
        _organisationFormalFrameworkId = _fixture.Create<Guid>();
        _formalFrameworkId = _fixture.Create<Guid>();
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
                _dateOfTermination.AddDays(_fixture.Create<int>())),
            new OrganisationLabelAdded(
                _organisationId,
                _fixture.Create<Guid>(),
                _fixture.Create<Guid>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _dateOfTermination.AddDays(_fixture.Create<int>() * -1),
                _dateOfTermination.AddDays(_fixture.Create<int>())),
            new OrganisationBecameActive(_organisationId), new OrganisationParentAdded(
                _organisationId,
                _organisationOrganisationParentId,
                _parentOrganisationId,
                _fixture.Create<string>(),
                null,
                null),
            new ParentAssignedToOrganisation(
                _organisationId,
                _parentOrganisationId,
                _organisationOrganisationParentId),
            new OrganisationFormalFrameworkAdded(
                _organisationId,
                _organisationFormalFrameworkId,
                _formalFrameworkId,
                _fixture.Create<string>(),
                _parentOrganisationId,
                _fixture.Create<string>(),
                null,
                null),
            new FormalFrameworkAssignedToOrganisation(
                _organisationId,
                _formalFrameworkId,
                _parentOrganisationId,
                _organisationFormalFrameworkId)
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
    public async Task PublishesThreeEvents()
    {
        await Given(Events).When(TerminateOrganisationCommand, UserBuilder.AlgemeenBeheerder())
            .ThenItPublishesTheCorrectNumberOfEvents(3);
    }

    [Fact]
    public async Task TerminatesTheOrganisation()
    {
        await Given(Events).When(TerminateOrganisationCommand, UserBuilder.AlgemeenBeheerder()).Then();
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
        organisationTerminated.FieldsToTerminate.FormalFrameworks.Should().HaveCount(1);
        organisationTerminated.FieldsToTerminate.OpeningHours.Should().BeEmpty();
        organisationTerminated.KboFieldsToTerminate.BankAccounts.Should().BeEmpty();
        organisationTerminated.DateOfTerminationAccordingToKbo.Should().BeNull();
    }

    [Fact]
    public async Task OrganisationBecomesInactive()
    {
        await Given(Events).When(TerminateOrganisationCommand, UserBuilder.AlgemeenBeheerder()).Then();

        var organisationBecameInactive = PublishedEvents[1].UnwrapBody<OrganisationBecameInactive>();
        organisationBecameInactive.Should().NotBeNull();

        organisationBecameInactive.OrganisationId.Should().Be(_organisationId);
    }

    [Fact]
    public void FormalFrameworkClearedFromOrganisation()
    {
        var frameworkClearedFromOrganisation = PublishedEvents[2].UnwrapBody<FormalFrameworkClearedFromOrganisation>();
        frameworkClearedFromOrganisation.Should().NotBeNull();

        frameworkClearedFromOrganisation.OrganisationId.Should().Be(_organisationId);
    }
}
