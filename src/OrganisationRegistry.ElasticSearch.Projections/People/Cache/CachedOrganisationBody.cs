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
    {
        return new CachedOrganisationBody(
            organisationPerBody.OrganisationId,
            organisationPerBody.OrganisationName);
    }

    public static CachedOrganisationBody Empty()
    {
        return new CachedOrganisationBody(
            null,
            string.Empty);
    }
}
