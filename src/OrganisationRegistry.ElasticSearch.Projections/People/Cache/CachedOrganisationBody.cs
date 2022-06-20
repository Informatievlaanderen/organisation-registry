namespace OrganisationRegistry.ElasticSearch.Projections.People.Cache;

using System;
using SqlServer.ElasticSearchProjections;

public class CachedOrganisationBody
{
    public Guid? OrganisationId { get; }
    public string OrganisationName { get; }

    private CachedOrganisationBody(Guid? organisationId, string organisationName)
    {
        OrganisationId = organisationId;
        OrganisationName = organisationName;
    }

    public static CachedOrganisationBody FromCache(OrganisationPerBody organisationPerBody)
        => new(
            organisationPerBody.OrganisationId,
            organisationPerBody.OrganisationName);

    public static CachedOrganisationBody Empty()
        => new(
            null,
            string.Empty);
}
