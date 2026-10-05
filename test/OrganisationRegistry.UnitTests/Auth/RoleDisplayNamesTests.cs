namespace OrganisationRegistry.UnitTests.Auth;

using FluentAssertions;
using OrganisationRegistry.Api.Auth.Models;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// <see cref="RoleDisplayNames.For"/> is the Dutch label shown as the "role"
/// field on the <c>/v1/me</c> response. Pins the exact copy per role, since the
/// frontend/UX is written against these literal strings (not the bare
/// <see cref="Role"/> enum name).
/// </summary>
public class RoleDisplayNamesTests
{
    [Theory]
    [InlineData(Role.VoMedewerker, "VO medewerker")]
    [InlineData(Role.AlgemeenBeheerder, "Algemeen beheerder")]
    [InlineData(Role.DecentraalBeheerder, "Decentraal beheerder")]
    [InlineData(Role.VlimpersBeheerder, "Vlimpers beheerder")]
    [InlineData(Role.OrgaanBeheerder, "Orgaan beheerder")]
    [InlineData(Role.RegelgevingBeheerder, "Regelgevingbeheerder en deugdelijk bestuur beheerder")]
    public void Maps_application_roles_to_their_Dutch_display_name(Role role, string expectedDisplayName)
        => RoleDisplayNames.For(role).Should().Be(expectedDisplayName);

    [Fact]
    public void Falls_back_to_the_bare_enum_name_for_a_role_without_a_mapping()
        => RoleDisplayNames.For(Role.CjmBeheerder).Should().Be("CjmBeheerder");
}
