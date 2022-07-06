namespace OrganisationRegistry.UnitTests;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Api.Security;
using AutoFixture;
using FluentAssertions;
using IdentityModel;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using Xunit;

public class OrganisationRegistryTokenBuilderTests
{
    [Theory]
    [InlineData("WegwijsBeheerder-vlimpersbeheerder:OVO001833")]
    [InlineData("WegwijsBeheerder-orgaanBeheerder:OVO001835")]
    [InlineData("WegwijsBeheerder-algemeenBeheerder:OVO002949")]
    public void BugFix_RoleOtherThanBeheerderShouldNotImplyBeheerderRole(string roleClaim)
    {
        var fixture = new Fixture();
        var tokenBuilder = new OrganisationRegistryTokenBuilder(new OpenIdConnectConfigurationSection());

        var claimsIdentity = tokenBuilder.ParseRoles(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(JwtClaimTypes.Subject, fixture.Create<string>()),
                    new Claim(AcmIdmConstants.Claims.Role, roleClaim),
                }));

        claimsIdentity.Claims
            .Where(claim => claim.Type == ClaimTypes.Role)
            .Select(claim => claim.Value)
            .ToList()
            .Should()
            .NotContain(Roles.DecentraalBeheerder);
    }

    [Fact]
    public void OrganisationRegistryBeheerdersDontGetOtherRoleClaims()
    {
        var claims = new[]
        {
            "WegwijsBeheerder-vlimpersbeheerder:OVO001833",
            "WegwijsBeheerder-algemeenBeheerder:OVO002949",
            "WegwijsBeheerder-decentraalBeheerder:OVO002949",
            "WegwijsBeheerder-orgaanBeheerder:OVO001835",
        };

        var fixture = new Fixture();
        var tokenBuilder = new OrganisationRegistryTokenBuilder(new OpenIdConnectConfigurationSection());

        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(JwtClaimTypes.Subject, fixture.Create<string>()));
        identity.AddClaims(claims.Select(claim => new Claim(AcmIdmConstants.Claims.Role, claim)));

        var claimsIdentity = tokenBuilder.ParseRoles(identity);

        claimsIdentity.Claims
            .Where(claim => claim.Type == ClaimTypes.Role)
            .Select(claim => claim.Value)
            .ToList()
            .Should()
            .BeEquivalentTo(Roles.AlgemeenBeheerder);
    }

    [Theory]
    [ClassData(typeof(RoleTestData))]
    public void ParsesRoles(string[] claims, string[] resultingRoleClaims)
    {
        var fixture = new Fixture();
        var tokenBuilder = new OrganisationRegistryTokenBuilder(new OpenIdConnectConfigurationSection());

        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(JwtClaimTypes.Subject, fixture.Create<string>()));
        identity.AddClaims(claims.Select(claim => new Claim(AcmIdmConstants.Claims.Role, claim)));

        var claimsIdentity = tokenBuilder.ParseRoles(identity);

        claimsIdentity.Claims
            .Where(claim => claim.Type == ClaimTypes.Role)
            .Select(claim => claim.Value)
            .ToList()
            .Should()
            .BeEquivalentTo(resultingRoleClaims);
    }

    [Theory]
    [ClassData(typeof(OrganisationClaimTestData))]
    public void ParsesOrganisationClaims(string[] claims, string[] resultingOrganisationClaims)
    {
        var fixture = new Fixture();
        var tokenBuilder = new OrganisationRegistryTokenBuilder(new OpenIdConnectConfigurationSection());

        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(JwtClaimTypes.Subject, fixture.Create<string>()));
        identity.AddClaims(claims.Select(claim => new Claim(AcmIdmConstants.Claims.Role, claim)));

        var claimsIdentity = tokenBuilder.ParseRoles(identity);

        claimsIdentity.Claims
            .Where(claim => claim.Type == AcmIdmConstants.Claims.Organisation)
            .Select(claim => claim.Value)
            .ToList()
            .Should()
            .BeEquivalentTo(resultingOrganisationClaims);
    }

    class RoleTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new[]
                {
                    "WegwijsBeheerder-vlimpersbeheerder:OVO001833",
                },
                new[]
                {
                    Roles.VlimpersBeheerder,
                },
            };
            yield return new object[]
            {
                new[]
                {
                    "WegwijsBeheerder-orgaanBeheerder:OVO001835",
                },
                new[]
                {
                    Roles.OrgaanBeheerder,
                },
            };
            yield return new object[]
            {
                new[]
                {
                    "WegwijsBeheerder-decentraalBeheerder:OVO002949",
                },
                new[]
                {
                    Roles.DecentraalBeheerder,
                },
            };
            yield return new object[]
            {
                new[]
                {
                    "WegwijsBeheerder-algemeenBeheerder:OVO002949",
                },
                new[]
                {
                    Roles.AlgemeenBeheerder,
                },
            };
            yield return new object[]
            {
                new[]
                {
                    "WegwijsBeheerder-vlimpersBeheerder:OVO002949",
                    "WegwijsBeheerder-orgaanBeheerder:OVO001835",
                },
                new[]
                {
                    Roles.VlimpersBeheerder,
                    Roles.OrgaanBeheerder,
                },
            };
            yield return new object[]
            {
                new[]
                {
                    "WegwijsBeheerder-regelgevingbeheerder:OVO002949",
                },
                new[]
                {
                    Roles.RegelgevingBeheerder,
                },
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class OrganisationClaimTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new[]
                {
                    "WegwijsBeheerder-vlimpersbeheerder:OVO001833",
                },
                Array.Empty<string>(),

            };
            yield return new object[]
            {
                new[]
                {
                    "WegwijsBeheerder-orgaanBeheerder:OVO001835",
                },
                Array.Empty<string>(),
            };
            yield return new object[]
            {
                new[]
                {
                    "WegwijsBeheerder-decentraalBeheerder:OVO002949",
                },
                new[]
                {
                    "ovo002949",
                },
            };
            yield return new object[]
            {
                new[]
                {
                    "WegwijsBeheerder-algemeenBeheerder:OVO002949",
                },
                Array.Empty<string>(),

            };
            yield return new object[]
            {
                new[]
                {
                    "WegwijsBeheerder-vlimpersBeheerder:OVO002949",
                    "WegwijsBeheerder-orgaanBeheerder:OVO001835",
                    "WegwijsBeheerder-decentraalBeheerder:OVO001833",
                    "WegwijsBeheerder-decentraalBeheerder:OVO001830",
                },
                new[]
                {
                    "ovo001833",
                    "ovo001830",
                },
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
