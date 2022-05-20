namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using Purpose;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Xunit;
using Xunit.Abstractions;

public class
    WhenTryingToChangeKboOwnedData : ExceptionOldSpecification2<UpdateOrganisationCommandHandler, UpdateOrganisationInfo>
{
    private readonly DateTime _yesterday = DateTime.Today.AddDays(-1);
    private Guid _organisationId;

    public WhenTryingToChangeKboOwnedData(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationCommandHandler>>().Object,
            Session,
            new DateTimeProviderStub(DateTime.Today));

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        var organisationCreatedBuilder = new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator());

        _organisationId = organisationCreatedBuilder.Id;

        return new List<IEvent>
        {
            organisationCreatedBuilder
                .WithValidity(null, null)
                .Build(),
            new OrganisationCoupledWithKbo(
                organisationCreatedBuilder.Id,
                "012313212",
                organisationCreatedBuilder.Name,
                "OVO999999",
                new DateTime()),
            new OrganisationBecameActive(organisationCreatedBuilder.Id)
        };
    }

    protected override UpdateOrganisationInfo When()
        => new(
            new OrganisationId(_organisationId),
            "Test",
            Article.None,
            "testing",
            "",
            new List<PurposeId>(),
            false,
            new ValidFrom(_yesterday),
            new ValidTo(_yesterday),
            new ValidFrom(),
            new ValidTo());

    protected override int ExpectedNumberOfEvents
        => 0;

    [Fact]
    public void TheOrganisationBecomesActive()
    {
        Exception.Should().BeOfType<CannotChangeDataOwnedByKbo>();
    }
}
