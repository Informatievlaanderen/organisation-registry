namespace OrganisationRegistry.ElasticSearch.Tests
{
    using System;
    using Xunit;

    public class EnvVarIgnoreFactAttribute : FactAttribute
    {
        public EnvVarIgnoreFactAttribute()
        {
            string envVar = "ES_TESTS";
            var env = Environment.GetEnvironmentVariable(envVar);
            if (string.IsNullOrEmpty(env))
                Skip = $"Please set {envVar} env var to run.";
        }
    }
}
