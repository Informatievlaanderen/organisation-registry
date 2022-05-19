namespace OrganisationRegistry.UnitTests.Organisation.TerminateOrganisation;

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
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using OrganisationRegistry.Organisation.State;
using Tests.Shared.Stubs;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class TerminateAlreadyTerminatedOrganisation : ExceptionSpecification<TerminateOrganisationCommandHandler, TerminateOrganisation>
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

    public TerminateAlreadyTerminatedOrganisation(ITestOutputHelper helper) : base(helper)
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
                null,
                null),
            OrganisationTerminated.Create(
                _organisationId,
                new OrganisationState(),
                new KboState(),
                new OrganisationTerminationSummaryBuilder().Build(),
                false,
                new OrganisationTerminationKboSummary(),
                fixture.Create<DateTime>())
        };
    }

    protected override TerminateOrganisation When()
        => new(
            _organisationId,
            _dateOfTermination,
            false);

    protected override TerminateOrganisationCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<TerminateOrganisationCommandHandler>>().Object,
            Session,
            _dateTimeProviderStub,
            _organisationRegistryConfigurationStub);


    protected override int ExpectedNumberOfEvents
        => 0;

    [Fact]
    public void ThrowsOrganisationAlreadyCoupledWithKbo()
    {
        Exception.Should().BeOfType<OrganisationAlreadyTerminated>();
    }
}
