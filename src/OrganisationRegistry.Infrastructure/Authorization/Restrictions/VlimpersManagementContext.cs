namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Minimal context that carries only whether the target organisation is under
/// Vlimpers management. Used by children/organisation restrictions that gate on
/// the Vlimpers-management flag (see
/// <see cref="RequireUnderVlimpersManagementRestriction"/> and
/// <see cref="RequireNotUnderVlimpersManagementRestriction"/>) without needing any
/// resource ids.
/// </summary>
public sealed record VlimpersManagementContext(bool IsUnderVlimpersManagement)
    : IVlimpersManagedContext
{
    public IEnumerable<Guid> RelevantIds => Enumerable.Empty<Guid>();
}
