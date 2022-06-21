namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
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
    WhenTryingToChangeKboOwnedData : Specification<UpdateOrganisationCommandHandler, UpdateOrganisationInfo>
{
    private readonly DateTime _yesterday;
    private readonly Guid _organisationId;
    private readonly string _organistationName;

    public WhenTryingToChangeKboOwnedData(ITestOutputHelper helper) : base(helper)
    {

        _organisationId = Guid.NewGuid();
        _yesterday = DateTime.Today.AddDays(-1);
        _organistationName = "Organisation Name";
    }

    protected override UpdateOrganisationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationCommandHandler>>().Object,
            session,
            new DateTimeProviderStub(DateTime.Today));

    private IEvent[] Events
        => new IEvent[] {
            new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator())
                .WithId(new OrganisationId(_organisationId))
                .WithName(_organistationName)
                .WithValidity(null, null)
                .Build(),
            new OrganisationCoupledWithKbo(
                _organisationId,
                "012313212",
                _organistationName,
                "OVO999999",
                new DateTime()),
            new OrganisationBecameActive(_organisationId),
        };

    private UpdateOrganisationInfo UpdateOrganisationInfoCommand
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

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(UpdateOrganisationInfoCommand, TestUser.AlgemeenBeheerder).ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task TheOrganisationBecomesActive()
    {
        await Given(Events).When(UpdateOrganisationInfoCommand, TestUser.AlgemeenBeheerder).ThenThrows<CannotChangeDataOwnedByKbo>();
    }
}
