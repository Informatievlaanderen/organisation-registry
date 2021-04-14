namespace OrganisationRegistry.ElasticSearch.Projections
{
    using System;
    using Microsoft.Extensions.Configuration;

    public static class IConfigurationExtensions
    {
        public static IConfigurationBuilder AddJsonFileIf(this IConfigurationBuilder builder, Func<bool> condition, string path, bool optional = false, bool reloadOnChange = false)
        {
            return condition() ? builder.AddJsonFile(path, optional, reloadOnChange) : builder;
        }
    }
}
