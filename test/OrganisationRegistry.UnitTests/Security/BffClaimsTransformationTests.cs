namespace OrganisationRegistry.UnitTests.Security;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Infrastructure.Security;
using Api.Security;
using FluentAssertions;
using IdentityModel;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// Tests voor BffClaimsTransformation: mapt claims uit de OAuth2 introspection response
/// (iv_wegwijs_rol_3D, given_name, family_name) naar interne ClaimTypes die door
/// SecurityService en [OrganisationRegistryAuthorize] gebruikt worden.
/// </summary>
public class BffClaimsTransformationTests
{
    private readonly BffClaimsTransformation _transformation = new();

    /// <summary>
    /// Simuleert een ClaimsIdentity zoals aangemaakt door de OAuth2Introspection library
    /// wanneer Startup.cs options.RoleClaimType = ClaimTypes.Role instelt (de fix).
    /// Zonder die fix zou RoleClaimType "role" zijn (korte JWT-vorm), waardoor
    /// IsInRole() nooit de door de transformatie toegevoegde claims zou vinden.
    /// </summary>
    private static ClaimsIdentity IntrospectionIdentity(params Claim[] claims)
        => new(claims, "BffApi", JwtClaimTypes.Name, ClaimTypes.Role);

    [Fact]
    public async Task WhenIdentityAlreadyHasClaimTypesGivenName_IsNotTransformed()
    {
        // Een JWT-identity (via Bearer scheme) heeft ClaimTypes.GivenName al gezet.
        // De transformatie mag die identity niet aanraken.
        var jwtIdentity = new ClaimsIdentity(
            new[]
            {
                new Claim(JwtClaimTypes.GivenName, "Alice"),
                new Claim(ClaimTypes.GivenName, "Alice"), // al aanwezig → geen introspection identity
            },
            "Bearer");
        var principal = new ClaimsPrincipal(jwtIdentity);

        var result = await _transformation.TransformAsync(principal);

        result.Should().BeSameAs(principal);
    }

    [Fact]
    public async Task WhenNoIdentityHasGivenName_PrincipalIsReturnedUnchanged()
    {
        var identity = new ClaimsIdentity(new[] { new Claim("sub", "abc") }, "BffApi");
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformation.TransformAsync(principal);

        result.Should().BeSameAs(principal);
    }

    [Fact]
    public async Task MapsGivenNameAndFamilyNameToClaimTypes()
    {
        var identity = IntrospectionIdentity(
            new Claim(JwtClaimTypes.GivenName, "Developer"),
            new Claim(JwtClaimTypes.FamilyName, "Persona"));
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformation.TransformAsync(principal);

        result.FindFirst(ClaimTypes.GivenName)!.Value.Should().Be("Developer");
        result.FindFirst(ClaimTypes.Surname)!.Value.Should().Be("Persona");
    }

    [Theory]
    [InlineData(AcmIdmConstants.Roles.AlgemeenBeheerder, Role.AlgemeenBeheerder)]
    [InlineData(AcmIdmConstants.Roles.VlimpersBeheerder, Role.VlimpersBeheerder)]
    [InlineData(AcmIdmConstants.Roles.OrgaanBeheerder, Role.OrgaanBeheerder)]
    [InlineData(AcmIdmConstants.Roles.CjmBeheerder, Role.CjmBeheerder)]
    [InlineData(AcmIdmConstants.Roles.RegelgevingBeheerder, Role.RegelgevingBeheerder)]
    [InlineData(AcmIdmConstants.Roles.DecentraalBeheerder, Role.DecentraalBeheerder)]
    public async Task MapsIvWegwijsRolClaimToClaimTypesRole(string acmRole, Role expectedRole)
    {
        var identity = IntrospectionIdentity(
            new Claim(JwtClaimTypes.GivenName, "Test"),
            new Claim(JwtClaimTypes.FamilyName, "User"),
            new Claim(AcmIdmConstants.Claims.Role, $"{AcmIdmConstants.RolePrefix}{acmRole}:OVO000001"));
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformation.TransformAsync(principal);

        result.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Should().Contain(RoleMapping.Map(expectedRole));
    }

    /// <summary>
    /// Regressietest voor de RoleClaimType-mismatch bug:
    /// OAuth2Introspection sette standaard RoleClaimType = "role" (korte JWT-naam),
    /// maar BffClaimsTransformation voegt rollen toe als ClaimTypes.Role (lange URI).
    /// IsInRole() zocht op RoleClaimType → vond nooit de toegevoegde claims → 403.
    /// Fix: options.RoleClaimType = ClaimTypes.Role in Startup.cs.
    /// Deze test simuleert dat door IntrospectionIdentity aan te maken met ClaimTypes.Role
    /// als RoleClaimType, precies zoals de library dat doet na de fix.
    /// </summary>
    [Theory]
    [InlineData(AcmIdmConstants.Roles.AlgemeenBeheerder, Role.AlgemeenBeheerder)]
    [InlineData(AcmIdmConstants.Roles.VlimpersBeheerder, Role.VlimpersBeheerder)]
    [InlineData(AcmIdmConstants.Roles.OrgaanBeheerder, Role.OrgaanBeheerder)]
    [InlineData(AcmIdmConstants.Roles.CjmBeheerder, Role.CjmBeheerder)]
    [InlineData(AcmIdmConstants.Roles.RegelgevingBeheerder, Role.RegelgevingBeheerder)]
    [InlineData(AcmIdmConstants.Roles.DecentraalBeheerder, Role.DecentraalBeheerder)]
    public async Task IsInRole_ReturnsTrueAfterTransformation(string acmRole, Role expectedRole)
    {
        var identity = IntrospectionIdentity(
            new Claim(JwtClaimTypes.GivenName, "Test"),
            new Claim(JwtClaimTypes.FamilyName, "User"),
            new Claim(AcmIdmConstants.Claims.Role, $"{AcmIdmConstants.RolePrefix}{acmRole}:OVO000001"));
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformation.TransformAsync(principal);

        result.IsInRole(RoleMapping.Map(expectedRole)).Should().BeTrue();
    }

    [Fact]
    public async Task WhenAlgemeenBeheerder_OtherRolesAreNotAdded()
    {
        // AlgemeenBeheerder is een superrol: als die aanwezig is, worden de anderen weggelaten
        var identity = IntrospectionIdentity(
            new Claim(JwtClaimTypes.GivenName, "Test"),
            new Claim(JwtClaimTypes.FamilyName, "User"),
            new Claim(AcmIdmConstants.Claims.Role, $"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.AlgemeenBeheerder}:OVO002949"),
            new Claim(AcmIdmConstants.Claims.Role, $"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.VlimpersBeheerder}:OVO001833"),
            new Claim(AcmIdmConstants.Claims.Role, $"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.DecentraalBeheerder}:OVO000001"));
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformation.TransformAsync(principal);

        result.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Should().ContainSingle()
            .Which.Should().Be(RoleMapping.Map(Role.AlgemeenBeheerder));
    }

    [Fact]
    public async Task DecentraalBeheerder_AddsOrganisationClaimsForEachOvo()
    {
        var identity = IntrospectionIdentity(
            new Claim(JwtClaimTypes.GivenName, "Test"),
            new Claim(JwtClaimTypes.FamilyName, "User"),
            new Claim(AcmIdmConstants.Claims.Role, $"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.DecentraalBeheerder}:OVO000001"),
            new Claim(AcmIdmConstants.Claims.Role, $"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.DecentraalBeheerder}:OVO000002"));
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformation.TransformAsync(principal);

        // De transformatie doet .ToLowerInvariant() op de claim value vóór de split,
        // dus OVO-nummers worden opgeslagen als lowercase.
        result.FindAll(AcmIdmConstants.Claims.Organisation)
            .Select(c => c.Value)
            .Should().BeEquivalentTo("ovo000001", "ovo000002");
    }

    [Fact]
    public async Task WhenNoMatchingRolePrefix_NoRoleClaimsAdded()
    {
        // iv_wegwijs_rol_3D claims van een ander systeem mogen geen rollen opleveren
        var identity = IntrospectionIdentity(
            new Claim(JwtClaimTypes.GivenName, "Test"),
            new Claim(JwtClaimTypes.FamilyName, "User"),
            new Claim(AcmIdmConstants.Claims.Role, "DienstverleningsRegister-admin:OVO002949"));
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformation.TransformAsync(principal);

        result.FindAll(ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public async Task WhenNoIvWegwijsRolClaims_NoRoleClaimsAdded()
    {
        var identity = IntrospectionIdentity(
            new Claim(JwtClaimTypes.GivenName, "Test"),
            new Claim(JwtClaimTypes.FamilyName, "User"));
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformation.TransformAsync(principal);

        result.FindAll(ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public async Task MultipleNonAlgemeenRoles_AreAllAdded()
    {
        var identity = IntrospectionIdentity(
            new Claim(JwtClaimTypes.GivenName, "Test"),
            new Claim(JwtClaimTypes.FamilyName, "User"),
            new Claim(AcmIdmConstants.Claims.Role, $"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.VlimpersBeheerder}:OVO001833"),
            new Claim(AcmIdmConstants.Claims.Role, $"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.OrgaanBeheerder}:OVO001835"),
            new Claim(AcmIdmConstants.Claims.Role, $"{AcmIdmConstants.RolePrefix}{AcmIdmConstants.Roles.DecentraalBeheerder}:OVO000001"));
        var principal = new ClaimsPrincipal(identity);

        var result = await _transformation.TransformAsync(principal);

        result.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Should().BeEquivalentTo(
                RoleMapping.Map(Role.VlimpersBeheerder),
                RoleMapping.Map(Role.OrgaanBeheerder),
                RoleMapping.Map(Role.DecentraalBeheerder));
    }
}
