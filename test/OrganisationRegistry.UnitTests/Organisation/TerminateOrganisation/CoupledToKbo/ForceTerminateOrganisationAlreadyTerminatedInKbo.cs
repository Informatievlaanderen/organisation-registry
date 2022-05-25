namespace OrganisationRegistry.UnitTests.Organisation.TerminateOrganisation.CoupledToKbo;

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
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class
    ForceTerminateOrganisationAlreadyTerminatedInKbo : Specification<TerminateOrganisationCommandHandler,
        TerminateOrganisation>
{
    private readonly OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub;

    private readonly Guid _organisationId;
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly DateTime _dateOfTermination;
    private readonly Fixture _fixture;

    public ForceTerminateOrganisationAlreadyTerminatedInKbo(ITestOutputHelper helper) : base(helper)
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
        => new IEvent[] {
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
            new OrganisationTerminationFoundInKbo(
                _organisationId,
                _fixture.Create<string>(),
                _fixture.Create<DateTime>(),
                _fixture.Create<string>(),
                _fixture.Create<string>())
        };

    private TerminateOrganisation TerminateOrganisationCommand
        => new(new OrganisationId(_organisationId), _dateOfTermination, true);

    protected override TerminateOrganisationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<TerminateOrganisationCommandHandler>>().Object,
            session,
            _dateTimeProviderStub,
            _organisationRegistryConfigurationStub);


    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(TerminateOrganisationCommand, UserBuilder.AlgemeenBeheerder())
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsOrganisationAlreadyCoupledWithKbo()
    {
        await Given(Events).When(TerminateOrganisationCommand, UserBuilder.AlgemeenBeheerder())
            .ThenThrows<OrganisationAlreadyTerminatedInKbo>();
    }
}
