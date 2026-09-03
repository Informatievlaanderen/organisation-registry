namespace OrganisationRegistry.Api.Security;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Authorization;

public static class ClaimsExtension
{
    /// <summary>
    /// Translates the role and scope claims on <paramref name="user"/> into the
    /// internal <see cref="PermissionSet"/> language.
    ///
    /// Reads both <see cref="ClaimTypes.Role"/> (post edge-translation) and the
    /// raw <see cref="AcmIdmConstants.Claims.Role"/> (pre edge-translation, with
    /// optional <see cref="AcmIdmConstants.RolePrefix"/> stripped) so the helper
    /// is safe to call at any stage of the auth pipeline. Scope claims are read
    /// from <see cref="AcmIdmConstants.Claims.Scope"/> and split on whitespace.
    ///
    /// Unknown roles and organisation-registry scopes fail closed (see
    /// <see cref="RolePermissionMap"/> and <see cref="ScopePermissionMap"/>).
    /// </summary>
    public static PermissionSet ToPermissionSet(this ClaimsPrincipal user, ILogger? logger = null)
    {
        var roles = user.GetClaims(ClaimTypes.Role)
            .Concat(user.GetClaims(AcmIdmConstants.Claims.Role))
            .Select(StripRolePrefix)
            .Where(RoleMapping.Exists)
            .Select(RoleMapping.Map)
            .ToArray();

        var scopes = user.GetClaims(AcmIdmConstants.Claims.Scope)
            .SelectMany(v => v.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .ToArray();

        return RolePermissionMap.For(roles, logger)
            .Union(ScopePermissionMap.For(scopes, logger));
    }

    private static string StripRolePrefix(string value)
        => value.StartsWith(AcmIdmConstants.RolePrefix, StringComparison.Ordinal)
            ? value[AcmIdmConstants.RolePrefix.Length..]
            : value;

    public static void AddOrUpdateClaim(this ClaimsIdentity identity, string key, Claim claim)
    {
        var existingClaim = identity.FindFirst(key);
        if (existingClaim != null)
            identity.RemoveClaim(existingClaim);

        identity.AddClaim(claim);
    }

    public static string? GetOptionalClaim(this ClaimsPrincipal user, string claimType)
        => user.Claims.SingleOrDefault(x => x.Type == claimType)?.Value;

    public static string? GetOptionalClaim(this ClaimsIdentity identity, string claimType)
        => identity.Claims.SingleOrDefault(x => x.Type == claimType)?.Value;

    public static string GetRequiredClaim(this ClaimsPrincipal user, string claimType)
        => user.Claims.Single(x => x.Type == claimType).Value;

    public static string GetRequiredClaim(this ClaimsIdentity identity, string claimType)
        => identity.Claims.Single(x => x.Type == claimType).Value;

    public static IEnumerable<string> GetClaims(this ClaimsPrincipal user, string claimType)
        => user.Claims.Where(x => x.Type == claimType).Select(x => x.Value);

    public static IEnumerable<string> GetClaims(this ClaimsIdentity identity, string claimType)
        => identity.Claims.Where(x => x.Type == claimType).Select(x => x.Value);
}
