namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationKey;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Commands;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class
    WhenRemovingAnOrganisationKeyToAnUncoupledKey :
        Specification<RemoveOrganisationKeyCommandHandler, RemoveOrganisationKey>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationKeyId;

    public WhenRemovingAnOrganisationKeyToAnUncoupledKey(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _organisationKeyId = Guid.NewGuid();
    }

    protected override RemoveOrganisationKeyCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<RemoveOrganisationKeyCommandHandler>>().Object,
            session);

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreated(
                _organisationId,
                "Kind en Gezin",
                "OVO000012345",
                "K&G",
                Article.None,
                "Kindjes en gezinnetjes",
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null)
        };

    private RemoveOrganisationKey RemoveOrganisationKeyCommand
        => new(
            new OrganisationId(_organisationId),
            new OrganisationKeyId(_organisationKeyId)
        );

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(RemoveOrganisationKeyCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }
}
