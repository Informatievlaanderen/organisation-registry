namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Marks a resource-scope context for restrictions. Implementations expose the
/// ids that are relevant for an authorization decision (e.g. keytype ids for a
/// Key command). Restrictions inspect a context through this non-generic marker
/// and fail closed when handed a context type they do not understand.
/// </summary>
public interface IRestrictionContext
{
    /// <summary>
    /// The ids that must all be allowed for the operation to be permitted.
    /// An empty enumerable means "nothing to check" and yields vacuous truth.
    /// </summary>
    IEnumerable<Guid> RelevantIds { get; }
}

/// <summary>
/// Capability a context exposes when it can report whether the organisation the
/// operation targets is under Vlimpers management. Restrictions that gate on the
/// Vlimpers-management flag (see
/// <see cref="RequireUnderVlimpersManagementRestriction"/>) require this
/// capability and reject contexts that do not carry it.
/// </summary>
public interface IVlimpersManagedContext : IRestrictionContext
{
    bool IsUnderVlimpersManagement { get; }
}
