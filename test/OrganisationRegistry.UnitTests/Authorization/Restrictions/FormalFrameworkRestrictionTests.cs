namespace OrganisationRegistry.UnitTests.Authorization.Restrictions;

using System;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using Tests.Shared;
using Xunit;

public class NotAllowListRestrictionTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void PassesWhenIdIsNotInDeniedList()
    {
        var denied = new[] { _fixture.Create<Guid>(), _fixture.Create<Guid>() };
        var contextId = _fixture.Create<Guid>();
        var restriction = new NotAllowListRestriction<FormalFrameworkContext>(denied);

        restriction.IsOkWith(new FormalFrameworkContext(contextId))
            .Should().BeTrue();
    }

    [Fact]
    public void FailsWhenIdIsInDeniedList()
    {
        var denied = new[] { _fixture.Create<Guid>(), _fixture.Create<Guid>() };
        var restriction = new NotAllowListRestriction<FormalFrameworkContext>(denied);

        restriction.IsOkWith(new FormalFrameworkContext(denied[0]))
            .Should().BeFalse();
    }

    [Fact]
    public void FailsClosedWhenContextIsMissing()
    {
        var restriction = new NotAllowListRestriction<FormalFrameworkContext>(Array.Empty<Guid>());

        restriction.IsOkWith()
            .Should().BeFalse();
    }

    [Fact]
    public void PassesWhenDeniedListIsEmpty()
    {
        var restriction = new NotAllowListRestriction<FormalFrameworkContext>(Array.Empty<Guid>());

        restriction.IsOkWith(new FormalFrameworkContext(_fixture.Create<Guid>()))
            .Should().BeTrue();
    }
}

public class DecentraalOrganisationRestrictionTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void PassesWhenUserIsDecentraalBeheerderForOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(ovoNumber)
            .Build();

        DecentraalOrganisationRestriction.Instance
            .IsOkWith(new UserContext(user), new OrganisationContext(ovoNumber))
            .Should().BeTrue();
    }

    [Fact]
    public void FailsWhenUserIsDecentraalBeheerderForDifferentOrganisation()
    {
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(_fixture.Create<string>())
            .Build();

        DecentraalOrganisationRestriction.Instance
            .IsOkWith(new UserContext(user), new OrganisationContext(_fixture.Create<string>()))
            .Should().BeFalse();
    }

    [Fact]
    public void FailsClosedWhenUserContextIsMissing()
    {
        DecentraalOrganisationRestriction.Instance
            .IsOkWith(new OrganisationContext(_fixture.Create<string>()))
            .Should().BeFalse();
    }

    [Fact]
    public void FailsClosedWhenOrganisationContextIsMissing()
    {
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(_fixture.Create<string>())
            .Build();

        DecentraalOrganisationRestriction.Instance
            .IsOkWith(new UserContext(user))
            .Should().BeFalse();
    }
}

public class FormalFrameworkRestrictionsTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void OwnedByVlimpersPassesForVlimpersId()
    {
        var vlimpersIds = new[] { _fixture.Create<Guid>() };
        var restriction = FormalFrameworkRestrictions.OwnedByVlimpers(vlimpersIds);

        restriction.IsOkWith(new FormalFrameworkContext(vlimpersIds[0]))
            .Should().BeTrue();
    }

    [Fact]
    public void OwnedByVlimpersFailsForNonVlimpersId()
    {
        var restriction = FormalFrameworkRestrictions.OwnedByVlimpers(new[] { _fixture.Create<Guid>() });

        restriction.IsOkWith(new FormalFrameworkContext(_fixture.Create<Guid>()))
            .Should().BeFalse();
    }

    [Fact]
    public void DecentraalOrganisationAndNotOwnedByVlimpersPassesForOwnOrganisationAndNonVlimpersId()
    {
        var ovoNumber = _fixture.Create<string>();
        var vlimpersIds = new[] { _fixture.Create<Guid>() };
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(ovoNumber)
            .Build();

        var restriction = FormalFrameworkRestrictions.DecentraalOrganisationAndNotOwnedByVlimpers(vlimpersIds);

        restriction.IsOkWith(
                new UserContext(user),
                new OrganisationContext(ovoNumber),
                new FormalFrameworkContext(_fixture.Create<Guid>()))
            .Should().BeTrue();
    }

    [Fact]
    public void DecentraalOrganisationAndNotOwnedByVlimpersFailsForVlimpersId()
    {
        var ovoNumber = _fixture.Create<string>();
        var vlimpersIds = new[] { _fixture.Create<Guid>() };
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(ovoNumber)
            .Build();

        var restriction = FormalFrameworkRestrictions.DecentraalOrganisationAndNotOwnedByVlimpers(vlimpersIds);

        restriction.IsOkWith(
                new UserContext(user),
                new OrganisationContext(ovoNumber),
                new FormalFrameworkContext(vlimpersIds[0]))
            .Should().BeFalse();
    }

    [Fact]
    public void DecentraalOrganisationAndNotOwnedByVlimpersFailsForOtherOrganisation()
    {
        var vlimpersIds = new[] { _fixture.Create<Guid>() };
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(_fixture.Create<string>())
            .Build();

        var restriction = FormalFrameworkRestrictions.DecentraalOrganisationAndNotOwnedByVlimpers(vlimpersIds);

        restriction.IsOkWith(
                new UserContext(user),
                new OrganisationContext(_fixture.Create<string>()),
                new FormalFrameworkContext(_fixture.Create<Guid>()))
            .Should().BeFalse();
    }
}
