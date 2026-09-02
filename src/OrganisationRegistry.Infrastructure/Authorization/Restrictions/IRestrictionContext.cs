namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Marks a resource-scope context for restrictions. Implementations expose the ids
/// that are relevant for authorization decisions (e.g. keytype ids for a Key
/// command) plus a static domain string that pairs the context with its entries
/// in a <see cref="PermissionSet"/>.
/// </summary>
public interface IRestrictionContext<TSelf>
    where TSelf : IRestrictionContext<TSelf>
{
    /// <summary>
    /// Stable domain identifier that links a context type to its restriction
    /// entries. Kept as a string so it survives serialization boundaries and
    /// is easy to grep. Values are PascalCase English (e.g. "OrganisationKeys").
    /// </summary>
    static abstract string Domain { get; }

    /// <summary>
    /// The ids that must all be allowed for the operation to be permitted.
    /// An empty enumerable means "nothing to check" and yields vacuous truth.
    /// </summary>
    IEnumerable<Guid> RelevantIds { get; }
}
