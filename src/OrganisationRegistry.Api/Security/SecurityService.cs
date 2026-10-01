namespace OrganisationRegistry.Api.Security;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Cache;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer;

public class SecurityService : ISecurityService
{
    private const string ClaimOrganisation = "urn:be:vlaanderen:wegwijs:organisation";

    private readonly ICache<OrganisationSecurityInformation> _cache;
    private readonly IOrganisationRegistryConfiguration _configuration;

    private readonly IContextFactory _contextFactory;

    public SecurityService(
        IContextFactory contextFactory,
        IOrganisationRegistryConfiguration configuration,
        ICache<OrganisationSecurityInformation> cache)
    {
        _contextFactory = contextFactory;
        _configuration = configuration;
        _cache = cache;
    }

    public async Task<SecurityInformation> GetSecurityInformation(ClaimsPrincipal? user)
    {
        if (user?.Identity == null || !user.Identity.IsAuthenticated)
            return SecurityInformation.None();

        var firstName = user.GetRequiredClaim(ClaimTypes.GivenName);
        var name = user.GetRequiredClaim(ClaimTypes.Surname);
        
        // Support both JWT Bearer (ACM-IDM) and OAuth2Introspection (TokenExchange) authentication
        string cacheKey;
        if (user.Identity.AuthenticationType == "OAuth2Introspection")
        {
            // For TokenExchange authentication, use vo_id as the cache key
            cacheKey = user.GetRequiredClaim("vo_id");
        }
        else
        {
            // For JWT Bearer authentication, use ACM ID as the cache key
            cacheKey = user.GetRequiredClaim(AcmIdmConstants.Claims.AcmId);
        }

        var roles = user
            .GetClaims(ClaimTypes.Role)
            .Where(RoleMapping.Exists)
            .Select(RoleMapping.Map)
            .ToImmutableArray();

        var organisationSecurityInformation = await _cache.GetOrAdd(
            cacheKey,
            async () =>
            {
                var organisations = GetOrganisations(user);
                return await GetSecurityInformation(organisations);
            });

        var permissions = RolePermissionMap.For(roles, _configuration);

        return new SecurityInformation(
            $"{firstName} {name}",
            roles,
            organisationSecurityInformation.OvoNumbers,
            organisationSecurityInformation.OrganisationIds,
            organisationSecurityInformation.BodyIds,
            permissions);
    }

    public async Task<IUser> GetRequiredUser(ClaimsPrincipal? principal)
    {
        if (principal == null)
            throw new Exception("Could not determine current user");

        var scopes = principal
            .FindAll(AcmIdmConstants.Claims.Scope)
            .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .ToImmutableHashSet();

        // Client-credential tokens (TestClient/Cjm/Orafin) carry no name/acmId
        // claims; their permissions are derived from the union of all scopes
        // on the token via ScopePermissionMap, which is the single source of
        // truth for scope → permission translation (see ClaimsExtension).
        if (scopes.Contains(AcmIdmConstants.Scopes.TestClient))
            return WellknownUsers.TestClient(ScopePermissionMap.For(scopes));

        if (scopes.Contains(AcmIdmConstants.Scopes.CjmBeheerder))
            return WellknownUsers.Cjm(ScopePermissionMap.For(scopes));

        if (scopes.Contains(AcmIdmConstants.Scopes.OrafinBeheerder))
            return WellknownUsers.Orafin(ScopePermissionMap.For(scopes));

        var firstName = principal.FindFirst(ClaimTypes.GivenName);
        if (firstName == null)
            throw new Exception("Could not determine current user's first name");

        var lastName = principal.FindFirst(ClaimTypes.Surname);
        if (lastName == null)
            throw new Exception("Could not determine current user's last name");

        string acmId;
        if (principal.Identity?.AuthenticationType == "OAuth2Introspection")
        {
            var voId = principal.FindFirst("vo_id");
            if (voId == null)
                throw new Exception("Could not determine current user's vo_id");
            acmId = voId.Value;
        }
        else
        {
            var acmIdClaim = principal.FindFirst(AcmIdmConstants.Claims.AcmId);
            if (acmIdClaim == null)
                throw new Exception("Could not determine current user's acm id");
            acmId = acmIdClaim.Value;
        }

        var ip = principal.FindFirst(AcmIdmConstants.Claims.Ip);

        var securityInformation = await GetSecurityInformation(principal);

        return new User(
            firstName.Value,
            lastName.Value,
            acmId,
            ip?.Value,
            securityInformation.Roles.ToArray(),
            securityInformation.OvoNumbers,
            securityInformation.BodyIds,
            securityInformation.OrganisationIds,
            RolePermissionMap.For(securityInformation.Roles, _configuration));
    }

    public async Task<IUser> GetUser(ClaimsPrincipal? principal)
    {
        if (principal == null || principal.Identity == null)
            throw new Exception("Could not determine current user");

        if (!principal.Identity.IsAuthenticated)
            return WellknownUsers.Nobody;

        var firstName = principal.GetRequiredClaim(ClaimTypes.GivenName);
        var lastName = principal.GetRequiredClaim(ClaimTypes.Surname);
        var acmId = principal.GetRequiredClaim(AcmIdmConstants.Claims.AcmId);
        var ip = principal.GetOptionalClaim(AcmIdmConstants.Claims.Ip);

        var securityInformation = await GetSecurityInformation(principal);

        return new User(
            firstName,
            lastName,
            acmId,
            ip,
            securityInformation.Roles.ToArray(),
            securityInformation.OvoNumbers,
            securityInformation.BodyIds,
            securityInformation.OrganisationIds,
            RolePermissionMap.For(securityInformation.Roles, _configuration));
    }

    public void ExpireUserCache(string acmId)
    {
        _cache.Expire(acmId);
    }

    private async Task<OrganisationSecurityInformation> GetSecurityInformation(ImmutableArray<string> ovoNumbers)
    {
        if (!ovoNumbers.Any())
            return new OrganisationSecurityInformation();

        await using var context = _contextFactory.Create();
        var organisationTrees =
            (await context
                .OrganisationTreeList
                .AsAsyncQueryable()
                .Where(x => ovoNumbers.Contains(x.OvoNumber))
                .Select(x => x.OrganisationTree ?? string.Empty)
                .ToListAsync())
            .SelectMany(x => x.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var organisationIds = await context
            .OrganisationDetail
            .AsAsyncQueryable()
            .Where(x => organisationTrees.Contains(x.OvoNumber))
            .Select(x => x.Id)
            .Distinct()
            .ToListAsync();

        var bodyIds = await context
            .ActiveBodyOrganisationList
            .AsAsyncQueryable()
            .Where(x => organisationIds.Contains(x.OrganisationId))
            .Select(x => x.BodyId)
            .Distinct()
            .ToListAsync();

        return new OrganisationSecurityInformation(organisationTrees, organisationIds, bodyIds);
    }

    private static ImmutableArray<string> GetOrganisations(ClaimsPrincipal user)
    {
        return user.GetClaims(ClaimOrganisation)
            .Select(s => s.ToUpperInvariant())
            .ToImmutableArray();
    }
}
