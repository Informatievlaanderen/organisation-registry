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
    WhenTryingToUpdateAVlimpersOrgAsNonVlimpersUser : Specification<UpdateOrganisationCommandHandler,
        UpdateOrganisationInfo>
{
    private readonly DateTime _yesterday;
    private readonly Guid _organisationId;

    public WhenTryingToUpdateAVlimpersOrgAsNonVlimpersUser(ITestOutputHelper helper) : base(helper)
    {
        _yesterday = DateTime.Today.AddDays(-1);
        _organisationId = Guid.NewGuid();
    }

    protected override UpdateOrganisationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationCommandHandler>>().Object,
            session,
            new DateTimeProviderStub(DateTime.Today));

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator())
                .WithId(new OrganisationId(_organisationId))
                .WithValidity(null, null)
                .Build(),
            new OrganisationBecameActive(_organisationId),
            new OrganisationPlacedUnderVlimpersManagement(_organisationId)
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
        await Given(Events).When(UpdateOrganisationInfoCommand, UserBuilder.VlimpersBeheerder())
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsAnException()
    {
        await Given(Events).When(UpdateOrganisationInfoCommand, UserBuilder.User()).ThenThrows<InsufficientRights>();
    }
}
