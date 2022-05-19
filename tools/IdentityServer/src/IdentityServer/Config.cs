using Duende.IdentityServer.Models;

namespace IdentityServer;

using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Test;
using IdentityModel;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource("vo", "Vlaamse Overheid", new []{"vo_id"}),
            new IdentityResource("iv_wegwijs", "Wegwijs", new []{"role", "iv_wegwijs_rol_3D"})

        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
        };

    public static IEnumerable<Client> Clients =>
        new []{
        new Client
        {
            ClientId = "organisation-registry-local-dev",
            ClientSecrets =
            {
                new Secret(
                    "a_very=Secr3t*Key".Sha256())
            },

            AllowedGrantTypes = GrantTypes.Code,

            // where to redirect to after login
            RedirectUris =
            {
                "https://organisatie.dev-vlaanderen.local/#/oic",
                "https://organisatie.dev-vlaanderen.local/v2/oic"
            },

            // where to redirect to after logout
            PostLogoutRedirectUris = { "https://organisatie.dev-vlaanderen.local" },
            FrontChannelLogoutUri = "https://organisatie.dev-vlaanderen.local",

            AllowedScopes = new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "vo",
                "iv_wegwijs"
            },
            AlwaysSendClientClaims = true,
            AlwaysIncludeUserClaimsInIdToken = true,
        }};

    public static List<TestUser> Users =>
        new List<TestUser>
        {
            new TestUser
            {
                Username = "dev",
                Password = "dev",
                IsActive = true,
                Claims = new List<Claim>
                {
                    new Claim("vo_id", "9C2F7372-7112-49DC-9771-F127B048B4C7"),
                    new Claim(JwtClaimTypes.FamilyName, "Persona"),
                    new Claim(JwtClaimTypes.GivenName, "Developer"),
                    new Claim("iv_wegwijs_rol_3D", "Dienstverleningsregister-admin:OVO002949"),
                    new Claim("iv_wegwijs_rol_3D", "Dienstverleningsregister-beheerder:OVO002949"),
                    new Claim("iv_wegwijs_rol_3D", "WegwijsBeheerder-beheerder:OVO000067"),
                    new Claim("iv_wegwijs_rol_3D", "WegwijsBeheerder-vlimpersbeheerder:OVO001833"),
                    new Claim("iv_wegwijs_rol_3D", "WegwijsBeheerder-algemeenbeheerder:OVO002949"),
                },
                SubjectId = "dev",
            },
            new TestUser
            {
                Username = "vlimpers",
                Password = "vlimpers",
                IsActive = true,
                Claims = new List<Claim>
                {
                    new Claim("vo_id", "E6D110DC-231A-4666-BAFB-C354255EF547"),
                    new Claim(JwtClaimTypes.FamilyName, "Persona"),
                    new Claim(JwtClaimTypes.GivenName, "Vlimpers"),
                    new Claim("iv_wegwijs_rol_3D", "WegwijsBeheerder-vlimpersbeheerder:OVO001833"),
                },
                SubjectId = "vlimpers",
            },
            new TestUser
            {
                Username = "decentraalbeheerder",
                Password = "decentraalbeheerder",
                IsActive = true,
                Claims = new List<Claim>
                {
                    new Claim("vo_id", "34E7CF51-0AF1-436E-B187-BEE803525BA6"),
                    new Claim(JwtClaimTypes.FamilyName, "Persona"),
                    new Claim(JwtClaimTypes.GivenName, "Decentraalbeheerder"),
                    new Claim("iv_wegwijs_rol_3D", "WegwijsBeheerder-decentraalbeheerder:OVO002949"),
                },
                SubjectId = "decentraalbeheerder",
            },
            new TestUser
            {
                Username = "algemeenbeheerder",
                Password = "algemeenbeheerder",
                IsActive = true,
                Claims = new List<Claim>
                {
                    new Claim("vo_id", "A5C5BFCD-266C-4CC7-9869-4B95E34C090D"),
                    new Claim(JwtClaimTypes.FamilyName, "Persona"),
                    new Claim(JwtClaimTypes.GivenName, "Algemeenbeheerder"),
                    new Claim("iv_wegwijs_rol_3D", "WegwijsBeheerder-algemeenbeheerder:OVO002949"),
                },
                SubjectId = "algemeenbeheerder",
            },
            new TestUser
            {
                Username = "organen",
                Password = "organen",
                IsActive = true,
                Claims = new List<Claim>
                {
                    new Claim("vo_id", "A5C5BFCD-266C-4CC7-9869-4B95E34C090D"),
                    new Claim(JwtClaimTypes.FamilyName, "Persona"),
                    new Claim(JwtClaimTypes.GivenName, "Orgaanbeheerder"),
                    new Claim("iv_wegwijs_rol_3D", "WegwijsBeheerder-orgaanbeheerder:OVO001835"),
                },
                SubjectId = "organen",
            },
            new TestUser
            {
                Username = "regelgeving",
                Password = "regelgeving",
                IsActive = true,
                Claims = new List<Claim>
                {
                    new Claim("vo_id", "A5C5BFCD-266C-4CC7-9869-4B95E34C090D"),
                    new Claim(JwtClaimTypes.FamilyName, "Persona"),
                    new Claim(JwtClaimTypes.GivenName, "Regelgevingbeheerder"),
                    new Claim("iv_wegwijs_rol_3D", "WegwijsBeheerder-regelgevingbeheerder:OVO001833"),
                },
                SubjectId = "regelgeving",
            }
        };
}
