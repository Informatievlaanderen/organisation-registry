namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationKey;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Handling.Authorization;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.KeyTypes.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Commands;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

/// <summary>
/// DecentraalBeheerder holds no CanManageKeys grant at all (restricted or
/// unrestricted) — key management is reserved for AlgemeenBeheerder and
/// (restricted, Vlimpers-only) VlimpersBeheerder. Removing a key must fail
/// regardless of Vlimpers-management status.
/// </summary>
public class WhenRemovingAnOrganisationKeyAsDecentraalBeheerder :
    Specification<RemoveOrganisationKeyCommandHandler, RemoveOrganisationKey>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationKeyId;
    private readonly Guid _keyId;

    public WhenRemovingAnOrganisationKeyAsDecentraalBeheerder(ITestOutputHelper helper) : base(helper)
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
    public async Task Fails()
    {
        await Given(Events)
            .When(RemoveOrganisationKeyCommand, TestUser.DecentraalBeheerder)
            .ThenThrows<InsufficientRights<KeyPolicy>>();
    }
}
