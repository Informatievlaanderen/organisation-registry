namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationKey;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Handling.Authorization;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
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
/// Regression tests for <see cref="RemoveOrganisationKeyCommandHandler"/>
/// using the restriction-aware <see cref="KeyPolicy"/> (via
/// <c>WithKeyPolicy(RemoveOrganisationKey)</c>) instead of the previous
/// coarse, context-blind <see cref="Permission.CanManageKeys"/> check.
/// VlimpersBeheerder only holds a restricted grant: it may only remove keys
/// whose keytype is Vlimpers-allowed AND whose organisation is under
/// Vlimpers management.
/// </summary>
public class WhenRemovingAnOrganisationKeyAsVlimpersBeheerder :
    Specification<RemoveOrganisationKeyCommandHandler, RemoveOrganisationKey>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationKeyId;
    private readonly Guid _keyId;
    private readonly Guid _otherKeyId;
    private readonly string _value;
    private readonly DateTime _validFrom;
    private readonly DateTime _validTo;

    public WhenRemovingAnOrganisationKeyAsVlimpersBeheerder(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();

        _keyId = Guid.NewGuid();
        _otherKeyId = Guid.NewGuid();
        _organisationKeyId = Guid.NewGuid();
        _value = "13135/123lk.,m";
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
    }

    protected override RemoveOrganisationKeyCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<RemoveOrganisationKeyCommandHandler>>().Object,
            session);

    private IEvent[] EventsFor(bool underVlimpersManagement)
    {
        var events = new List<IEvent>
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
            new KeyTypeCreated(_otherKeyId, "Sleutel B"),
            new OrganisationKeyAdded(
                _organisationId,
                _organisationKeyId,
                _keyId,
                "Sleutel A",
                _value,
                _validFrom,
                _validTo),
        };

        if (underVlimpersManagement)
            events.Add(new OrganisationPlacedUnderVlimpersManagement(_organisationId));

        return events.ToArray();
    }

    private RemoveOrganisationKey RemoveOrganisationKeyCommand
        => new(
            new OrganisationId(_organisationId),
            new OrganisationKeyId(_organisationKeyId));

    private User VlimpersBeheerderAllowedFor(params Guid[] allowedKeyTypeIds)
        => new UserBuilder()
            .AddRoles(Role.VlimpersBeheerder)
            .WithPermissions(
                PermissionSet.Of(
                    Permission.CanManageKeys.RestrictedTo(
                        KeyRestrictions.VlimpersManaged(allowedKeyTypeIds))))
            .Build();

    [Fact]
    public async Task SucceedsWhenUnderVlimpersManagementAndKeyTypeAllowed()
    {
        await Given(EventsFor(underVlimpersManagement: true))
            .When(RemoveOrganisationKeyCommand, VlimpersBeheerderAllowedFor(_keyId))
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task FailsWhenNotUnderVlimpersManagement()
    {
        await Given(EventsFor(underVlimpersManagement: false))
            .When(RemoveOrganisationKeyCommand, VlimpersBeheerderAllowedFor(_keyId))
            .ThenThrows<InsufficientRights<KeyPolicy>>();
    }

    [Fact]
    public async Task FailsWhenKeyTypeNotAllowed()
    {
        await Given(EventsFor(underVlimpersManagement: true))
            .When(RemoveOrganisationKeyCommand, VlimpersBeheerderAllowedFor(_otherKeyId))
            .ThenThrows<InsufficientRights<KeyPolicy>>();
    }
}
