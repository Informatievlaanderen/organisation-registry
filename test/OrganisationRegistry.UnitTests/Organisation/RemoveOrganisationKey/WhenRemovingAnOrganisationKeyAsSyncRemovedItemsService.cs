namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationKey;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.KeyTypes.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Commands;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

/// <summary>
/// The only caller of <see cref="RemoveOrganisationKey"/> is the
/// SyncRemovedItemsService background job, acting as
/// <see cref="WellknownUsers.SyncRemovedItemsService"/>. That well-known user
/// must hold an unrestricted <see cref="Permission.CanManageKeys"/> grant (it
/// isn't tied to any role/organisation), otherwise scheduled key removals
/// would always fail authorization — regardless of whether the handler uses
/// the coarse or the restriction-aware check.
/// </summary>
public class WhenRemovingAnOrganisationKeyAsSyncRemovedItemsService :
    Specification<RemoveOrganisationKeyCommandHandler, RemoveOrganisationKey>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationKeyId;
    private readonly Guid _keyId;

    public WhenRemovingAnOrganisationKeyAsSyncRemovedItemsService(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _keyId = Guid.NewGuid();
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
                null),
            new KeyTypeCreated(_keyId, "Sleutel A"),
            new OrganisationKeyAdded(
                _organisationId,
                _organisationKeyId,
                _keyId,
                "Sleutel A",
                "13135/123lk.,m",
                DateTime.Now.AddDays(1),
                DateTime.Now.AddDays(2)),
        };

    private RemoveOrganisationKey RemoveOrganisationKeyCommand
        => new(
            new OrganisationId(_organisationId),
            new OrganisationKeyId(_organisationKeyId));

    [Fact]
    public async Task Succeeds()
    {
        await Given(Events)
            .When(RemoveOrganisationKeyCommand, WellknownUsers.SyncRemovedItemsService)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }
}
