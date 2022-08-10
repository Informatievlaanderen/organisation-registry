namespace OrganisationRegistry.UnitTests.Security;

using System.Linq;
using System.Security.Claims;
using Api.Security;
using FluentAssertions;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using Xunit;

public class OrganisationRegistryTokenBuilderTests
{
    [Theory]
    [InlineData($"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.CjmBeheerder}:OVO000001", Role.CjmBeheerder)]
    [InlineData($"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.AlgemeenBeheerder}:OVO000001", Role.AlgemeenBeheerder)]
    [InlineData($"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.DecentraalBeheerder}:OVO000001", Role.DecentraalBeheerder)]
    [InlineData($"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.RegelgevingBeheerder}:OVO000001", Role.RegelgevingBeheerder)]
    [InlineData($"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.OrgaanBeheerder}:OVO000001", Role.OrgaanBeheerder)]
    [InlineData($"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.VlimpersBeheerder}:OVO000001", Role.VlimpersBeheerder)]
    public void ParsesAcmRoleClaims(string rawClaimValue, Role expectedRole)
    {
        var identityFromAcm = new ClaimsIdentity();
        identityFromAcm.AddClaim(new Claim(AcmIdmConstants.Claims.Role, rawClaimValue));

        var builder = new OrganisationRegistryTokenBuilder(new OpenIdConnectConfigurationSection());

        var identity = builder.ParseRoles(identityFromAcm);

        var roles = identity.FindAll(claim => claim.Type.Equals(ClaimTypes.Role)).ToList();

        roles.Should().HaveCount(1);
        roles[0].Value.Should().Be(RoleMapping.Map(expectedRole));
    }

    [Fact]
    public void ForDecentraalBeheerders_AddsOrganisationClaims()
    {
        var ovoNumber = "ovo000001";
        var ovoNumber2 = "ovo000002";

        var identityFromAcm = new ClaimsIdentity();
        identityFromAcm.AddClaim(new Claim(AcmIdmConstants.Claims.Role, $"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.DecentraalBeheerder}:{ovoNumber}"));
        identityFromAcm.AddClaim(new Claim(AcmIdmConstants.Claims.Role, $"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.DecentraalBeheerder}:{ovoNumber2}"));

        var builder = new OrganisationRegistryTokenBuilder(new OpenIdConnectConfigurationSection());

        var identity = builder.ParseRoles(identityFromAcm);

        var roles = identity.FindAll(claim => claim.Type.Equals(AcmIdmConstants.Claims.Organisation)).ToList();

        roles.Should().HaveCount(2);
        roles[0].Value.Should().Be(ovoNumber);
        roles[1].Value.Should().Be(ovoNumber2);
    }
}
