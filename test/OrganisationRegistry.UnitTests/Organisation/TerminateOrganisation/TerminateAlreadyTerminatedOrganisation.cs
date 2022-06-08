namespace OrganisationRegistry.UnitTests.Organisation.TerminateOrganisation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using OrganisationRegistry.Organisation.State;
using Tests.Shared;
using Tests.Shared.Stubs;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class
    TerminateAlreadyTerminatedOrganisation : Specification<TerminateOrganisationCommandHandler, TerminateOrganisation>
{
    private readonly OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub;

    private readonly Guid _organisationId;
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly DateTime _dateOfTermination;
    private readonly Fixture _fixture;

    public TerminateAlreadyTerminatedOrganisation(ITestOutputHelper helper) : base(helper)
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
                null,
                null),
#pragma warning disable CS0618
            OrganisationTerminated.Create(
#pragma warning restore CS0618
                _organisationId,
                new OrganisationState(),
                new KboState(),
                new OrganisationTerminationSummaryBuilder().Build(),
                false,
                new OrganisationTerminationKboSummary(),
                _fixture.Create<DateTime>())
        };

    private TerminateOrganisation TerminateOrganisationCommand
        => new(
            new OrganisationId(_organisationId),
            _dateOfTermination,
            false);

    protected override TerminateOrganisationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<TerminateOrganisationCommandHandler>>().Object,
            session,
            _dateTimeProviderStub,
            _organisationRegistryConfigurationStub);


    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(TerminateOrganisationCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsOrganisationAlreadyTerminated()
    {
        await Given(Events).When(TerminateOrganisationCommand, TestUser.AlgemeenBeheerder)
            .ThenThrows<OrganisationAlreadyTerminated>();
    }
}
