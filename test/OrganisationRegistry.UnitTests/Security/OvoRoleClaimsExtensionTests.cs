namespace OrganisationRegistry.UnitTests.Security;

using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using OrganisationRegistry.Api.Security;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

public class OvoRoleClaimsExtensionTests
{
    private const string Prefix = AcmIdmConstants.RolePrefix;

    private static ClaimsPrincipal PrincipalWith(params string[] roleClaimValues)
        => new(IdentityWith(roleClaimValues));

    private static ClaimsIdentity IdentityWith(params string[] roleClaimValues)
        => new(roleClaimValues.Select(v => new Claim(AcmIdmConstants.Claims.Role, v)));

    // Realistic set: one group with one role, one group with two roles.
    private static ClaimsPrincipal TypicalUser()
        => PrincipalWith(
            $"{Prefix}algemeenbeheerder:OVO002949",
            $"{Prefix}vomedewerker,regelgevingbeheerder:OVO001833");

    public class RolesByOvoCode
    {
        [Fact]
        public void NoClaims_ReturnsEmpty()
            => PrincipalWith().RolesByOvoCode.Should().BeEmpty();

        [Fact]
        public void SingleRole_SingleGroup()
        {
            var result = PrincipalWith($"{Prefix}algemeenbeheerder:OVO002949").RolesByOvoCode;

            result.Keys.Should().BeEquivalentTo("ovo002949");
            result["ovo002949"].Should().BeEquivalentTo("algemeenbeheerder");
        }

        [Fact]
        public void MultipleRoles_AreAllAttachedToTheOvoCode()
        {
            var result = PrincipalWith($"{Prefix}vomedewerker,regelgevingbeheerder:OVO001833").RolesByOvoCode;

            result.Keys.Should().BeEquivalentTo("ovo001833");
            result["ovo001833"].Should().BeEquivalentTo("vomedewerker", "regelgevingbeheerder");
        }

        [Fact]
        public void MultipleGroups_AreKeptSeparate()
        {
            var result = TypicalUser().RolesByOvoCode;

            result.Keys.Should().BeEquivalentTo("ovo002949", "ovo001833");
            result["ovo002949"].Should().BeEquivalentTo("algemeenbeheerder");
            result["ovo001833"].Should().BeEquivalentTo("vomedewerker", "regelgevingbeheerder");
        }

        [Fact]
        public void SameOvoCodeInMultipleClaims_IsMerged()
        {
            var result = PrincipalWith(
                $"{Prefix}vomedewerker:OVO001833",
                $"{Prefix}regelgevingbeheerder:OVO001833").RolesByOvoCode;

            result.Keys.Should().BeEquivalentTo("ovo001833");
            result["ovo001833"].Should().BeEquivalentTo("vomedewerker", "regelgevingbeheerder");
        }

        [Fact]
        public void DuplicateRoles_AreDeduplicated()
        {
            var result = PrincipalWith(
                $"{Prefix}vomedewerker,vomedewerker:OVO001833",
                $"{Prefix}vomedewerker:OVO001833").RolesByOvoCode;

            result["ovo001833"].Should().HaveCount(1);
        }

        [Fact]
        public void PrefixIsOptional()
        {
            var result = PrincipalWith("vomedewerker,regelgevingbeheerder:OVO001833").RolesByOvoCode;

            result["ovo001833"].Should().BeEquivalentTo("vomedewerker", "regelgevingbeheerder");
        }

        [Fact]
        public void PrefixIsMatchedCaseInsensitive()
        {
            var result = PrincipalWith("WegwijsBeheerder-AlgemeenBeheerder:OVO002949").RolesByOvoCode;

            result.Keys.Should().BeEquivalentTo("ovo002949");
            result["ovo002949"].Should().BeEquivalentTo("algemeenbeheerder");
        }

        [Fact]
        public void Values_AreNormalisedToLowerCase()
        {
            var result = PrincipalWith($"{Prefix}AlgemeenBeheerder:OVO002949").RolesByOvoCode;

            result.Keys.Single().Should().Be("ovo002949");
            result["ovo002949"].Single().Should().Be("algemeenbeheerder");
        }

        [Fact]
        public void Lookup_IsCaseInsensitive()
        {
            var result = PrincipalWith($"{Prefix}algemeenbeheerder:OVO002949").RolesByOvoCode;

            result.ContainsKey("OVO002949").Should().BeTrue();
            result["OVO002949"].Contains("AlgemeenBeheerder").Should().BeTrue();
        }

        [Fact]
        public void Whitespace_IsTrimmed()
        {
            var result = PrincipalWith($"  {Prefix}vomedewerker , regelgevingbeheerder : OVO001833  ").RolesByOvoCode;

            result.Keys.Should().BeEquivalentTo("ovo001833");
            result["ovo001833"].Should().BeEquivalentTo("vomedewerker", "regelgevingbeheerder");
        }

        [Fact]
        public void EmptyEntriesBetweenCommas_AreIgnored()
        {
            var result = PrincipalWith($"{Prefix}vomedewerker,,regelgevingbeheerder,:OVO001833").RolesByOvoCode;

            result["ovo001833"].Should().BeEquivalentTo("vomedewerker", "regelgevingbeheerder");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("algemeenbeheerder")] // no ':'
        [InlineData("wegwijsbeheerder-algemeenbeheerder")] // prefix, but no ':'
        [InlineData("wegwijsbeheerder-algemeenbeheerder:")] // no ovo-code
        [InlineData("wegwijsbeheerder-algemeenbeheerder:   ")] // blank ovo-code
        [InlineData("wegwijsbeheerder-:OVO002949")] // no roles
        [InlineData("wegwijsbeheerder-,,:OVO002949")] // only empty roles
        [InlineData(":OVO002949")] // no prefix, no roles
        [InlineData("wegwijsbeheerder-:")] // nothing at all
        [InlineData(":")]
        public void MalformedValue_IsIgnored_AndDoesNotThrow(string malformed)
        {
            var act = () => PrincipalWith(malformed).RolesByOvoCode;

            act.Should().NotThrow();
            act().Should().BeEmpty();
        }

        [Fact]
        public void MalformedValue_DoesNotAffectValidValues()
        {
            var result = PrincipalWith(
                "garbage",
                $"{Prefix}algemeenbeheerder:OVO002949",
                $"{Prefix}:OVO000000",
                $"{Prefix}vomedewerker:").RolesByOvoCode;

            result.Keys.Should().BeEquivalentTo("ovo002949");
            result["ovo002949"].Should().BeEquivalentTo("algemeenbeheerder");
        }

        [Fact]
        public void OtherClaimTypes_AreIgnored()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "algemeenbeheerder:OVO002949"),
                new Claim("sub", "someone"),
            });

            new ClaimsPrincipal(identity).RolesByOvoCode.Should().BeEmpty();
        }

        [Fact]
        public void ClaimsIdentity_Overload_ParsesTheSame()
        {
            var result = IdentityWith(
                $"{Prefix}algemeenbeheerder:OVO002949",
                $"{Prefix}vomedewerker,regelgevingbeheerder:OVO001833").RolesByOvoCode;

            result.Keys.Should().BeEquivalentTo("ovo002949", "ovo001833");
            result["ovo001833"].Should().BeEquivalentTo("vomedewerker", "regelgevingbeheerder");
        }

        [Fact]
        public void ClaimsIdentity_WithoutClaims_ReturnsEmpty()
            => new ClaimsIdentity().RolesByOvoCode.Should().BeEmpty();

        [Fact]
        public void PrincipalWithMultipleIdentities_CombinesTheirClaims()
        {
            var principal = new ClaimsPrincipal(new[]
            {
                IdentityWith($"{Prefix}algemeenbeheerder:OVO002949"),
                IdentityWith($"{Prefix}vomedewerker:OVO001833"),
            });

            principal.RolesByOvoCode.Keys.Should().BeEquivalentTo("ovo002949", "ovo001833");
        }
    }

    public class GetRoles
    {
        [Fact]
        public void ReturnsRolesOfThatOvoCode()
            => TypicalUser().GetRoles("ovo001833").Should().BeEquivalentTo("vomedewerker", "regelgevingbeheerder");

        [Fact]
        public void IsCaseInsensitive()
            => TypicalUser().GetRoles("OVO002949").Should().BeEquivalentTo("algemeenbeheerder");

        [Fact]
        public void UnknownOvoCode_ReturnsEmpty()
            => TypicalUser().GetRoles("ovo999999").Should().BeEmpty();

        [Fact]
        public void NoClaims_ReturnsEmpty()
            => PrincipalWith().GetRoles("ovo001833").Should().BeEmpty();
    }

    public class GetOvoCodes
    {
        [Fact]
        public void ReturnsOvoCodesWhereRoleIsHeld()
            => TypicalUser().GetOvoCodes("vomedewerker").Should().BeEquivalentTo("ovo001833");

        [Fact]
        public void RoleInMultipleGroups_ReturnsAllOvoCodes()
        {
            var user = PrincipalWith(
                $"{Prefix}decentraalbeheerder:OVO000001",
                $"{Prefix}decentraalbeheerder:OVO000002",
                $"{Prefix}vomedewerker:OVO000003");

            user.GetOvoCodes("decentraalbeheerder").Should().BeEquivalentTo("ovo000001", "ovo000002");
        }

        [Fact]
        public void IsCaseInsensitive()
            => TypicalUser().GetOvoCodes("AlgemeenBeheerder").Should().BeEquivalentTo("ovo002949");

        [Fact]
        public void ResultIsCaseInsensitiveSet()
            => TypicalUser().GetOvoCodes("algemeenbeheerder").Contains("OVO002949").Should().BeTrue();

        [Fact]
        public void UnknownRole_ReturnsEmpty()
            => TypicalUser().GetOvoCodes("orgaanbeheerder").Should().BeEmpty();

        [Fact]
        public void NoClaims_ReturnsEmpty()
            => PrincipalWith().GetOvoCodes("algemeenbeheerder").Should().BeEmpty();
    }

    public class HasRole
    {
        [Theory]
        [InlineData("algemeenbeheerder")]
        [InlineData("vomedewerker")]
        [InlineData("regelgevingbeheerder")]
        [InlineData("REGELGEVINGBEHEERDER")]
        public void InAnyOvoCode_WhenRoleIsHeld_ReturnsTrue(string role)
            => TypicalUser().HasRole(role).Should().BeTrue();

        [Fact]
        public void InAnyOvoCode_WhenRoleIsNotHeld_ReturnsFalse()
            => TypicalUser().HasRole("orgaanbeheerder").Should().BeFalse();

        [Fact]
        public void InSpecifiedOvoCode_WhenHeldThere_ReturnsTrue()
            => TypicalUser().HasRole("vomedewerker", "ovo001833").Should().BeTrue();

        [Fact]
        public void InSpecifiedOvoCode_IsCaseInsensitive()
            => TypicalUser().HasRole("vomedewerker", "OVO001833").Should().BeTrue();

        [Fact]
        public void InSpecifiedOvoCode_WhenHeldInOtherOvoCodeOnly_ReturnsFalse()
            => TypicalUser().HasRole("vomedewerker", "ovo002949").Should().BeFalse();

        [Fact]
        public void InSeveralSpecifiedOvoCodes_WhenHeldInOne_ReturnsTrue()
            => TypicalUser().HasRole("vomedewerker", "ovo002949", "ovo001833").Should().BeTrue();

        [Fact]
        public void InSeveralSpecifiedOvoCodes_WhenHeldInNone_ReturnsFalse()
            => TypicalUser().HasRole("vomedewerker", "ovo002949", "ovo999999").Should().BeFalse();

        [Fact]
        public void InUnknownOvoCode_ReturnsFalse()
            => TypicalUser().HasRole("algemeenbeheerder", "ovo999999").Should().BeFalse();

        [Fact]
        public void NoClaims_ReturnsFalse()
        {
            PrincipalWith().HasRole("algemeenbeheerder").Should().BeFalse();
            PrincipalWith().HasRole("algemeenbeheerder", "ovo002949").Should().BeFalse();
        }

        [Fact]
        public void MalformedClaim_GrantsNothing()
            => PrincipalWith("algemeenbeheerder").HasRole("algemeenbeheerder").Should().BeFalse();
    }

    public class HasOvoCode
    {
        [Theory]
        [InlineData("ovo002949")]
        [InlineData("ovo001833")]
        [InlineData("OVO001833")]
        public void WithAnyRole_WhenMember_ReturnsTrue(string ovoCode)
            => TypicalUser().HasOvoCode(ovoCode).Should().BeTrue();

        [Fact]
        public void WithAnyRole_WhenNotMember_ReturnsFalse()
            => TypicalUser().HasOvoCode("ovo999999").Should().BeFalse();

        [Fact]
        public void WithSpecifiedRole_WhenHeldThere_ReturnsTrue()
            => TypicalUser().HasOvoCode("ovo001833", "regelgevingbeheerder").Should().BeTrue();

        [Fact]
        public void WithSpecifiedRole_IsCaseInsensitive()
            => TypicalUser().HasOvoCode("ovo001833", "RegelgevingBeheerder").Should().BeTrue();

        [Fact]
        public void WithSpecifiedRole_WhenHeldInOtherOvoCodeOnly_ReturnsFalse()
            => TypicalUser().HasOvoCode("ovo002949", "vomedewerker").Should().BeFalse();

        [Fact]
        public void WithSeveralSpecifiedRoles_WhenOneMatches_ReturnsTrue()
            => TypicalUser().HasOvoCode("ovo001833", "algemeenbeheerder", "vomedewerker").Should().BeTrue();

        [Fact]
        public void WithSeveralSpecifiedRoles_WhenNoneMatches_ReturnsFalse()
            => TypicalUser().HasOvoCode("ovo001833", "algemeenbeheerder", "orgaanbeheerder").Should().BeFalse();

        [Fact]
        public void NoClaims_ReturnsFalse()
        {
            PrincipalWith().HasOvoCode("ovo002949").Should().BeFalse();
            PrincipalWith().HasOvoCode("ovo002949", "algemeenbeheerder").Should().BeFalse();
        }

        [Fact]
        public void MalformedClaim_GrantsNothing()
            => PrincipalWith($"{Prefix}algemeenbeheerder:").HasOvoCode("ovo002949").Should().BeFalse();
    }
}
