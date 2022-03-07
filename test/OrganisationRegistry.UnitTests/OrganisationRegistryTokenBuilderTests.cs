namespace OrganisationRegistry.UnitTests
{
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
                        new Claim(OrganisationRegistryClaims.ClaimRoles, roleClaim)
                    }));

            claimsIdentity.Claims
                .Where(claim => claim.Type == ClaimTypes.Role)
                .Select(claim => claim.Value)
                .ToList()
                .Should()
                .NotContain(Roles.OrganisatieBeheerder);
        }

        [Fact]
        public void OrganisationRegistryBeheerdersDontGetOtherRoleClaims()
        {
            var claims = new[]
            {
                "WegwijsBeheerder-vlimpersbeheerder:OVO001833",
                "WegwijsBeheerder-algemeenBeheerder:OVO002949",
                "WegwijsBeheerder-beheerder:OVO002949",
                "WegwijsBeheerder-orgaanBeheerder:OVO001835"
            };

            var fixture = new Fixture();
            var tokenBuilder = new OrganisationRegistryTokenBuilder(new OpenIdConnectConfigurationSection());

            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(JwtClaimTypes.Subject, fixture.Create<string>()));
            identity.AddClaims(claims.Select(claim => new Claim(OrganisationRegistryClaims.ClaimRoles, claim)));

            var claimsIdentity = tokenBuilder.ParseRoles(identity);

            claimsIdentity.Claims
                .Where(claim => claim.Type == ClaimTypes.Role)
                .Select(claim => claim.Value)
                .ToList()
                .Should()
                .BeEquivalentTo(Roles.OrganisationRegistryBeheerder);
        }

        [Theory]
        [ClassData(typeof(RoleTestData))]
        public void ParsesRoles(string[] claims, string[] resultingRoleClaims)
        {
            var fixture = new Fixture();
            var tokenBuilder = new OrganisationRegistryTokenBuilder(new OpenIdConnectConfigurationSection());

            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(JwtClaimTypes.Subject, fixture.Create<string>()));
            identity.AddClaims(claims.Select(claim => new Claim(OrganisationRegistryClaims.ClaimRoles, claim)));

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
            identity.AddClaims(claims.Select(claim => new Claim(OrganisationRegistryClaims.ClaimRoles, claim)));

            var claimsIdentity = tokenBuilder.ParseRoles(identity);

            claimsIdentity.Claims
                .Where(claim => claim.Type == OrganisationRegistryClaims.ClaimOrganisation)
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
                        Roles.VlimpersBeheerder
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
                        Roles.OrgaanBeheerder
                    },
                };
                yield return new object[]
                {
                    new[]
                    {
                        "WegwijsBeheerder-beheerder:OVO002949",
                    },
                    new[]
                    {
                        Roles.OrganisatieBeheerder
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
                        Roles.OrganisationRegistryBeheerder
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
                        Roles.OrgaanBeheerder
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
                        "WegwijsBeheerder-beheerder:OVO002949",
                    },
                    new[]
                    {
                        "ovo002949"
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
                        "WegwijsBeheerder-Beheerder:OVO001833",
                        "WegwijsBeheerder-Beheerder:OVO001830",
                    },
                    new[]
                    {
                        "ovo001833",
                        "ovo001830"
                    },
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}