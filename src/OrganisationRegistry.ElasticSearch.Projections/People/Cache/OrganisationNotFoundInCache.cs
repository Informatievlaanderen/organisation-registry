namespace OrganisationRegistry.ElasticSearch.Projections.People.Cache;

using System;

public class OrganisationNotFoundInCache : Exception
{
    public OrganisationNotFoundInCache(Guid organisationId, string cacheName):base($"Organisation {organisationId} not found in cache {cacheName}")
    {
    }
}
