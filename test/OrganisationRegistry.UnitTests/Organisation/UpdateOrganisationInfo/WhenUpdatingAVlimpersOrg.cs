namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Handling.Authorization;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using Purpose;
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

public class
    WhenUpdatingAVlimpersOrg : Specification<UpdateOrganisationCommandHandler,
        UpdateOrganisationInfo>
{
    private readonly DateTime _yesterday;
    private readonly Guid _organisationId;

    public WhenUpdatingAVlimpersOrg(ITestOutputHelper helper) : base(helper)
    {
        _yesterday = DateTime.Today.AddDays(-1);
        _organisationId = Guid.NewGuid();
    }

    protected override UpdateOrganisationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationCommandHandler>>().Object,
            session,
            new DateTimeProviderStub(DateTime.Today));

    // CanManageOrganisation is a config-driven restricted grant for
    // VlimpersBeheerder (see RolePermissionMap.RestrictedGrantsFor), so the
    // test user needs permissions derived via the config-aware overload;
    // TestUser.VlimpersBeheerder (config-less) never carries it.
    private static IUser VlimpersBeheerderUser()
    {
        var configuration = new OrganisationRegistryConfigurationStub();
        return new UserBuilder()
            .AddRoles(Role.VlimpersBeheerder)
            .WithPermissions(RolePermissionMap.For(new[] { Role.VlimpersBeheerder }, configuration))
            .Build();
    }

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator())
                .WithId(new OrganisationId(_organisationId))
                .WithValidity(null, null)
                .Build(),
            new OrganisationBecameActive(_organisationId),
            new OrganisationPlacedUnderVlimpersManagement(_organisationId),
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
    public async Task AsVlimpersBeheerderItPublishesEvents()
    {
        await Given(Events).When(UpdateOrganisationInfoCommand, VlimpersBeheerderUser())
            .ThenItPublishesTheCorrectNumberOfEvents(6);
    }

    [Fact]
    public async Task AsNonVlimpersUserItThrowsAnException()
    {
        await Given(Events).When(UpdateOrganisationInfoCommand, TestUser.User).ThenThrows<InsufficientRights<OrganisationPolicy>>();
    }
}
