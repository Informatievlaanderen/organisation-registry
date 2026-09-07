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
    public IEnumerable<Guid> RelevantIds => new[] { OrganisationClassificationTypeId };
}
