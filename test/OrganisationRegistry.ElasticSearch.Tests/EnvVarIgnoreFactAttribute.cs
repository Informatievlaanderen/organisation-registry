namespace OrganisationRegistry.ElasticSearch.Tests
{
    using System;
    using Xunit;

    public class EnvVarIgnoreFactAttribute : FactAttribute
    {
        public EnvVarIgnoreFactAttribute()
        {
            const string envVar = "IGNORE_ES_TESTS";
            var env = Environment.GetEnvironmentVariable(envVar);
            if (!string.IsNullOrEmpty(env) && env.Trim().ToUpper() != "FALSE")
                Skip = $"Ignored because {envVar} env var is set.";
        }
    }
}
