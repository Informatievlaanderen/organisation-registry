namespace OrganisationRegistry.Tests.Shared;

using System;
using Xunit;

public sealed class EnvVarIgnoreTheoryAttribute : TheoryAttribute
{
    public EnvVarIgnoreTheoryAttribute()
    {
        const string envVar = "IGNORE_EDIT_API_TESTS";
        var env = Environment.GetEnvironmentVariable(envVar);
        if (!string.IsNullOrEmpty(env))
            Skip = $"Ignored because {envVar} env var is set.";
    }
}
