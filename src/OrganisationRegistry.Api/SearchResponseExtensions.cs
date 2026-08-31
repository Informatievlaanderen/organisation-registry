namespace OrganisationRegistry.Api;

using ElasticSearch.Common;
using Infrastructure;
using Microsoft.Extensions.Logging;
using Osc;

public static class SearchResponseExtensions
{
    public static ISearchResponse<T> ThrowIfInvalid<T>(this ISearchResponse<T> searchResults, ILogger logger) where T : class
    {
        if (!searchResults.IsValid)
        {
            const string logMessage = "Er is een probleem opgetreden bij het uitvoeren van de zoekopdracht.";

            logger.LogCritical(logMessage + " {Error}", searchResults.FormatError());

            throw searchResults.ServerError?.Error?.Type == "search_phase_execution_exception"
                // throw searchResults.Hits.Count.Equals(0) && string.IsNullOrEmpty(searchResults.ScrollId) // Parameters for identifying a timed out scroll
                ? new ElasticsearchScrollTimeoutException(logMessage)
                : new ApiException(logMessage);
        }

        return searchResults;
    }
}
