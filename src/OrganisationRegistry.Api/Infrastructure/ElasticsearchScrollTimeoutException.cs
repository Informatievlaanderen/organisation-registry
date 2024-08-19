namespace OrganisationRegistry.Api.Infrastructure;

using System;

public class ElasticsearchScrollTimeoutException : DomainException
{
    public ElasticsearchScrollTimeoutException()
    {
    }

    public ElasticsearchScrollTimeoutException(string message) : base(message)
    {
    }

    public ElasticsearchScrollTimeoutException(string message, Exception inner) : base(message, inner)
    {
    }
}
