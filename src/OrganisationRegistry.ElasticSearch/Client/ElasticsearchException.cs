namespace OrganisationRegistry.ElasticSearch.Client;

using System;

public class ElasticsearchException : Exception
{
    protected ElasticsearchException() { }

    protected ElasticsearchException(string message) : base(message) { }

    public ElasticsearchException(string message, Exception inner) : base(message, inner) { }
}
