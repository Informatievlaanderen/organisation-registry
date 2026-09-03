namespace OrganisationRegistry.Infrastructure.Authorization;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides just-in-time, per-request access to a user's scope restrictions —
/// the resource identifiers (OVO-numbers, organisation ids, body ids) that
/// bound which resources a permission may act on.
///
/// Restrictions are NOT part of a <see cref="PermissionSet"/>: the permission
/// answers "is this action allowed at all?" and restrictions answer
/// "on which resources?". Permission checks live in controllers via
/// <c>[OrganisationRegistryAuthorize(RequiredPermissions=...)]</c>; scope
/// restrictions are resolved inside <c>ISecurityPolicy</c> implementations
/// when they need to validate a target resource.
///
/// Implementations MUST be registered with a request-scoped lifetime and
/// SHOULD memoise the underlying projection lookup for the duration of the
/// request to keep controller-to-policy calls cheap.
/// </summary>
public interface IUserRestrictionsProvider
{
    /// <summary>
    /// Returns the OVO-numbers of organisations the current user has been
    /// granted DecentraalBeheerder-style access to. Empty when the user has
    /// no scope restrictions (either unrestricted admin or unauthenticated).
    /// </summary>
    Task<IReadOnlyCollection<string>> GetOvoNumbersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the organisation ids the current user has been granted
    /// DecentraalBeheerder-style access to.
    /// </summary>
    Task<IReadOnlyCollection<Guid>> GetOrganisationIdsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the body ids the current user has been granted
    /// OrgaanBeheerder-style access to.
    /// </summary>
    Task<IReadOnlyCollection<Guid>> GetBodyIdsAsync(CancellationToken cancellationToken = default);
}
