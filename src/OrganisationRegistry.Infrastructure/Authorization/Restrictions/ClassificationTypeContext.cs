namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;

/// <summary>
/// Context for OrganisationClassification operations. Carries the
/// organisationclassificationtype id a command touches so the
/// OrganisationClassificationTypePolicy can decide against the caller's allowed
/// classificationtype set (e.g. the Regelgeving- or Cjm-owned types).
/// </summary>
public sealed record ClassificationTypeContext(Guid OrganisationClassificationTypeId) : IRestrictionContext
{
    private readonly IReadOnlyCollection<Guid>? _organisationClassificationTypeIds;

    /// <summary>
    /// Multi-id / "any" overload used for organisation-level permission summaries:
    /// an empty collection yields an empty <see cref="RelevantIds"/> (vacuous truth).
    /// </summary>
    public ClassificationTypeContext(IReadOnlyCollection<Guid> organisationClassificationTypeIds) : this(Guid.Empty)
        => _organisationClassificationTypeIds = organisationClassificationTypeIds;

    public IEnumerable<Guid> RelevantIds => _organisationClassificationTypeIds ?? new[] { OrganisationClassificationTypeId };
}
