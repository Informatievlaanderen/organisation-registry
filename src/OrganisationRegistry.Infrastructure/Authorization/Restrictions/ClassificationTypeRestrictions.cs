namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Convenience factories for common OrganisationClassificationType restrictions.
/// </summary>
public static class ClassificationTypeRestrictions
{
    /// <summary>
    /// The classificationtype id must be in the Regelgeving DB-owned set.
    /// </summary>
    public static IRestriction OwnedByRegelgevingDb(IEnumerable<Guid> classificationTypeIds)
        => new AllowListRestriction<ClassificationTypeContext>(classificationTypeIds);

    /// <summary>
    /// The classificationtype id must be in the CJM-owned set.
    /// </summary>
    public static IRestriction OwnedByCjm(IEnumerable<Guid> classificationTypeIds)
        => new AllowListRestriction<ClassificationTypeContext>(classificationTypeIds);

    /// <summary>
    /// DecentraalBeheerder grant: own organisation and the classificationtype id
    /// must be neither Regelgeving DB-owned.
    /// </summary>
    public static IRestriction DecentraalOrganisationAndNotOwned(
        IEnumerable<Guid> regelgevingDbClassificationTypeIds,
        IEnumerable<Guid> cjmClassificationTypeIds)
        => new CompositeAndRestriction(
            DecentraalOrganisationRestriction.Instance,
            new NotAllowListRestriction<ClassificationTypeContext>(
                regelgevingDbClassificationTypeIds.Concat(cjmClassificationTypeIds)));
}
