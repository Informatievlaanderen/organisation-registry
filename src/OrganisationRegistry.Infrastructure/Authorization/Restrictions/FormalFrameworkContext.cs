namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Context for OrganisationFormalFramework operations. Carries the formal
/// framework id(s) a command touches and whether the target organisation is
/// under Vlimpers management, so the FormalFrameworkPolicy can decide against
/// the caller's allowed formal-framework set and the Vlimpers-management gate
/// in one pass. Mirrors <see cref="KeyContext"/> and <see cref="LabelContext"/>.
///
/// Note: the Vlimpers-management gate only applies to the VlimpersBeheerder
/// grant (see <see cref="FormalFrameworkRestrictions.VlimpersManaged"/>); other
/// restricted grants (e.g. RegelgevingBeheerder's own formal-framework set) are
/// not organisation-scoped and simply ignore <see cref="IsUnderVlimpersManagement"/>.
/// </summary>
public sealed record FormalFrameworkContext(bool IsUnderVlimpersManagement, IReadOnlyCollection<Guid> FormalFrameworkIds)
    : IVlimpersManagedContext
{
    public FormalFrameworkContext(bool isUnderVlimpersManagement, Guid formalFrameworkId)
        : this(isUnderVlimpersManagement, new[] { formalFrameworkId }) { }

    public IEnumerable<Guid> RelevantIds => FormalFrameworkIds;
}
