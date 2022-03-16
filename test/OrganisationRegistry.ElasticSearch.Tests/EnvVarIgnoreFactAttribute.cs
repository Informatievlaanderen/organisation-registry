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
            if (!string.IsNullOrEmpty(env) && !string.Equals(env, "false", StringComparison.InvariantCultureIgnoreCase))
                Skip = $"Ignored because {envVar} env var is set.";
        }
    }
}
