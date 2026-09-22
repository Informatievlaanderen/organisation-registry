namespace OrganisationRegistry.UnitTests.Auth;

using FluentAssertions;
using OrganisationRegistry.Api.Auth.Models;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

public class RolePriorityTests
{
    [Fact]
    public void Selects_Developer_above_all_others()
        => RolePriority.SelectPrimary(new[]
            {
                Role.AlgemeenBeheerder,
                Role.OrgaanBeheerder,
                Role.Developer,
                Role.VlimpersBeheerder,
            })
            .Should().Be(Role.Developer);

    [Fact]
    public void Selects_AlgemeenBeheerder_above_all_others()
        => RolePriority.SelectPrimary(new[]
            {
                Role.OrgaanBeheerder,
                Role.DecentraalBeheerder,
                Role.AlgemeenBeheerder,
                Role.VlimpersBeheerder,
            })
            .Should().Be(Role.AlgemeenBeheerder);

    [Fact]
    public void Prefers_DecentraalBeheerder_above_VlimpersBeheerder()
        => RolePriority.SelectPrimary(new[] { Role.VlimpersBeheerder, Role.DecentraalBeheerder })
            .Should().Be(Role.DecentraalBeheerder);

    [Fact]
    public void Prefers_RegelgevingBeheerder_above_OrgaanBeheerder()
        => RolePriority.SelectPrimary(new[] { Role.OrgaanBeheerder, Role.RegelgevingBeheerder })
            .Should().Be(Role.RegelgevingBeheerder);

    [Fact]
    public void Follows_the_full_priority_order()
        => RolePriority.SelectPrimary(new[]
            {
                Role.Orafin,
                Role.OrgaanBeheerder,
                Role.RegelgevingBeheerder,
                Role.VlimpersBeheerder,
                Role.DecentraalBeheerder,
            })
            .Should().Be(Role.DecentraalBeheerder);

    [Fact]
    public void Never_selects_CjmBeheerder_as_primary_role()
        => RolePriority.SelectPrimary(new[] { Role.CjmBeheerder, Role.OrgaanBeheerder })
            .Should().Be(Role.OrgaanBeheerder);

    [Fact]
    public void Returns_null_when_user_has_no_application_role()
        => RolePriority.SelectPrimary(new[] { Role.CjmBeheerder, Role.AutomatedTask })
            .Should().BeNull();

    [Fact]
    public void Returns_null_for_an_empty_role_set()
        => RolePriority.SelectPrimary(System.Array.Empty<Role>())
            .Should().BeNull();
}
