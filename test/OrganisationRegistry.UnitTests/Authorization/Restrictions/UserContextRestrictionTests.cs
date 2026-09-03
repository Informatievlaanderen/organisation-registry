namespace OrganisationRegistry.UnitTests.Authorization.Restrictions;

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using Tests.Shared;
using Xunit;

public class UserContextRestrictionTests
{
    [Fact]
    public void DecentraalBeheerderForOrganisationRestriction_passes_when_user_owns_organisation()
    {
        var organisationId = Guid.NewGuid();
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisationIds(organisationId)
            .Build();

        var restriction = new DecentraalBeheerderForOrganisationRestriction(organisationId);

        restriction.IsOkWith(new UserContext(user), new LabelContext(false, "OVO000001", Array.Empty<Guid>()))
            .Should().BeTrue();
    }

    [Fact]
    public void DecentraalBeheerderForOrganisationRestriction_fails_when_user_does_not_own_organisation()
    {
        var organisationId = Guid.NewGuid();
        var otherOrganisationId = Guid.NewGuid();
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisationIds(otherOrganisationId)
            .Build();

        var restriction = new DecentraalBeheerderForOrganisationRestriction(organisationId);

        restriction.IsOkWith(new UserContext(user), new LabelContext(false, "OVO000001", Array.Empty<Guid>()))
            .Should().BeFalse();
    }

    [Fact]
    public void DecentraalBeheerderForOrganisationRestriction_fails_when_user_is_not_decentraal_beheerder()
    {
        var organisationId = Guid.NewGuid();
        var user = new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .AddOrganisationIds(organisationId)
            .Build();

        var restriction = new DecentraalBeheerderForOrganisationRestriction(organisationId);

        restriction.IsOkWith(new UserContext(user), new LabelContext(false, "OVO000001", Array.Empty<Guid>()))
            .Should().BeFalse();
    }

    [Fact]
    public void DecentraalBeheerderForOrganisationRestriction_fails_closed_without_user_context()
    {
        var organisationId = Guid.NewGuid();
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisationIds(organisationId)
            .Build();

        var restriction = new DecentraalBeheerderForOrganisationRestriction(organisationId);

        // No UserContext supplied
        restriction.IsOkWith(new LabelContext(false, "OVO000001", Array.Empty<Guid>()))
            .Should().BeFalse();
    }

    private sealed class DecentraalBeheerderForOrganisationRestriction : IRestriction
    {
        private readonly Guid _organisationId;

        public DecentraalBeheerderForOrganisationRestriction(Guid organisationId)
            => _organisationId = organisationId;

        public bool IsOkWith(params IRestrictionContext[] contexts)
        {
            var userContext = contexts.OfType<UserContext>().FirstOrDefault();
            if (userContext is null)
                return false;

            return userContext.User.IsDecentraalBeheerderForOrganisation(_organisationId);
        }
    }

    private sealed record LabelContext(
        bool IsUnderVlimpersManagement,
        string OvoNumber,
        IReadOnlyCollection<Guid> LabelTypeIds) : IRestrictionContext
    {
        public IEnumerable<Guid> RelevantIds => LabelTypeIds;
    }
}
