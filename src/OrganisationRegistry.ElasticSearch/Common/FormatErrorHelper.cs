namespace OrganisationRegistry.ElasticSearch.Common
{
    using Osc;

    public static class FormatErrorHelper
    {
        public static string FormatError(this IResponse response)
        {
            //ServerError ==> error on elastichsearch side
            //OriginalException ==> error on elastichsearch client side
            //DebugInformation ==> human readable string representation of what happened on both successful and failed request

            return $"ServerError: {response.ServerError}\n" +
                   $"OriginalException: {response.OriginalException?.Message}\n" +
                   $"DebugInformation: {response.DebugInformation}";
        }
    }
}
