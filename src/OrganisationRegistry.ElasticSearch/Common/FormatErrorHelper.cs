namespace OrganisationRegistry.ElasticSearch.Common;

using OpenSearch.Client;

public static class FormatErrorHelper
{
    //ServerError ==> error on elastichsearch side
    //OriginalException ==> error on elastichsearch client side
    //DebugInformation ==> human readable string representation of what happened on both successful and failed request
    public static string FormatError(this IResponse response)
        => $"ServerError: {response.ServerError}\n" +
           $"OriginalException: {response.OriginalException?.Message}\n" +
           $"DebugInformation: {response.DebugInformation}";
}
