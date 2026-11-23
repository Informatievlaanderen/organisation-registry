namespace OrganisationRegistry.Api.Security;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using OrganisationRegistry.Infrastructure.Authorization;

/// <summary>
/// Roles grouped by OVO-code, parsed from the <see cref="AcmIdmConstants.Claims.Role"/> claims.
///
/// Claim format (one claim per group / OVO-code):
///   [WegwijsBeheerder-]role1,role2,...:OVOxxxxxx
///
/// Values are normalised to lower case; lookups are case-insensitive.
/// Malformed values (no ':', no roles or no OVO-code) are ignored (fail closed).
/// </summary>
public static class OvoRoleClaimsExtension
{
    private static readonly IReadOnlySet<string> NoValues = new HashSet<string>();

    extension(ClaimsPrincipal user)
    {
        /// <summary>All roles per OVO-code, e.g. { "ovo001833": { "vomedewerker", "regelgevingbeheerder" } }.</summary>
        public IReadOnlyDictionary<string, IReadOnlySet<string>> RolesByOvoCode
            => Parse(user.GetClaims(AcmIdmConstants.Claims.Role));

        /// <summary>The roles the user has within one OVO-code.</summary>
        public IReadOnlySet<string> GetRoles(string ovoCode)
            => user.RolesByOvoCode.TryGetValue(ovoCode, out var roles) ? roles : NoValues;

        /// <summary>The OVO-codes in which the user has the given role.</summary>
        public IReadOnlySet<string> GetOvoCodes(string role)
            => user.RolesByOvoCode
                .Where(x => x.Value.Contains(role))
                .Select(x => x.Key)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

        /// <summary>Has the role in any OVO-code, or - when <paramref name="ovoCodes"/> is given - in at least one of those.</summary>
        public bool HasRole(string role, params string[] ovoCodes)
            => user.RolesByOvoCode.Any(x =>
                x.Value.Contains(role)
                && (ovoCodes.Length == 0 || ovoCodes.Contains(x.Key, StringComparer.OrdinalIgnoreCase)));

        /// <summary>Has at least one of the roles, in any OVO-code.</summary>
        public bool HasAnyRole(params string[] roles)
            => user.RolesByOvoCode.Any(x => roles.Any(x.Value.Contains));

        /// <summary>Is a member of the OVO-code with any role, or - when <paramref name="roles"/> is given - with at least one of those.</summary>
        public bool HasOvoCode(string ovoCode, params string[] roles)
            => user.RolesByOvoCode.TryGetValue(ovoCode, out var found)
               && (roles.Length == 0 || roles.Any(found.Contains));
    }

    extension(ClaimsIdentity identity)
    {
        public IReadOnlyDictionary<string, IReadOnlySet<string>> RolesByOvoCode
            => Parse(identity.GetClaims(AcmIdmConstants.Claims.Role));
    }

    private static Dictionary<string, IReadOnlySet<string>> Parse(IEnumerable<string> claimValues)
    {
        var result = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var claimValue in claimValues)
        {
            if (!TryParse(claimValue, out var ovoCode, out var roles))
                continue;

            if (!result.TryGetValue(ovoCode, out var set))
                result[ovoCode] = set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var role in roles)
                set.Add(role);
        }

        return result.ToDictionary(
            x => x.Key,
            x => (IReadOnlySet<string>)x.Value,
            StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Parses one claim value ("[WegwijsBeheerder-]role1,role2:OVOxxxxxx") into a lower-cased
    /// OVO-code and its roles. Returns false for malformed values.
    /// </summary>
    private static bool TryParse(
        string? claimValue,
        [NotNullWhen(true)] out string? ovoCode,
        [NotNullWhen(true)] out string[]? roles)
    {
        ovoCode = null;
        roles = null;

        if (string.IsNullOrWhiteSpace(claimValue))
            return false;

        var value = claimValue.Trim().ToLowerInvariant();

        if (value.StartsWith(AcmIdmConstants.RolePrefix, StringComparison.Ordinal))
            value = value[AcmIdmConstants.RolePrefix.Length..];

        var separator = value.LastIndexOf(':');
        if (separator < 0)
            return false;

        var code = value[(separator + 1)..].Trim();
        var parsedRoles = value[..separator]
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (code.Length == 0 || parsedRoles.Length == 0)
            return false;

        ovoCode = code;
        roles = parsedRoles;
        return true;
    }
}
