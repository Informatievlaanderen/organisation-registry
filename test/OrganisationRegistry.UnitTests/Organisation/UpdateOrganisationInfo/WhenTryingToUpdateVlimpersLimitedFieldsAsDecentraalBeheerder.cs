namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo;

using System;
using System.Threading.Tasks;
using Handling.Authorization;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

/// <summary>
/// DecentraalBeheerder holds a restricted <see cref="Permission.CanManageOrganisationInfoNotLimitedToVlimpers"/>
/// grant for their own organisation, but that must never extend to the four
/// Vlimpers-reserved fields (formele naam, formele korte naam, lidwoord,
/// operationele geldigheid) gated by
/// <see cref="Permission.CanManageOrganisationInfoLimitedToVlimpers"/> — a
/// permission DecentraalBeheerder never holds, regardless of the organisation's
/// Vlimpers-management status or ownership.
/// </summary>
public class WhenTryingToUpdateVlimpersLimitedFieldsAsDecentraalBeheerder :
    Specification<UpdateOrganisationInfoLimitedToVlimpersCommandHandler, UpdateOrganisationInfoLimitedToVlimpers>
{
    private Guid _organisationId;
    private string _ovoNumber = null!;

    // Config-aware permissions, so the restricted
    // CanManageOrganisationInfoNotLimitedToVlimpers grant for
    // DecentraalBeheerder's own organisation is present, proving that even
    // holding that grant does not extend to the Vlimpers-reserved fields.
    private IUser DecentraalBeheerderUser()
    {
        var configuration = new OrganisationRegistryConfigurationStub();
        return new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(_ovoNumber)
            .WithPermissions(RolePermissionMap.For(new[] { Role.DecentraalBeheerder }, configuration))
            .Build();
    }

    public WhenTryingToUpdateVlimpersLimitedFieldsAsDecentraalBeheerder(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationInfoLimitedToVlimpersCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationInfoLimitedToVlimpersCommandHandler>>().Object,
            session,
            new DateTimeProviderStub(DateTime.Today));

    private IEvent[] Events()
    {
        var organisationCreatedBuilder = new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator());

        _organisationId = organisationCreatedBuilder.Id;
        _ovoNumber = organisationCreatedBuilder.OvoNumber;

        return new IEvent[]
        {
            organisationCreatedBuilder
                .WithValidity(null, null)
                .Build(),
            new OrganisationBecameActive(organisationCreatedBuilder.Id),
        };
    }

    private UpdateOrganisationInfoLimitedToVlimpers UpdateOrganisationInfoLimitedToVlimpersCommand
        => new(
            new OrganisationId(_organisationId),
            "Test",
            Article.None,
            "testing",
            new ValidFrom(),
            new ValidTo(),
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task ItThrowsAnException()
    {
        var events = Events();
        await Given(events).When(UpdateOrganisationInfoLimitedToVlimpersCommand, DecentraalBeheerderUser())
            .ThenThrows<InsufficientRights<OrganisationPolicy>>();
    }

    [Fact]
    public async Task PublishesNoEvents()
    {
        var events = Events();
        await Given(events).When(UpdateOrganisationInfoLimitedToVlimpersCommand, DecentraalBeheerderUser())
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }
}
