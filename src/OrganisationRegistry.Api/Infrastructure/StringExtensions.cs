namespace OrganisationRegistry.Api.Infrastructure.OrganisationRegistryConfiguration
{
    using System;
    using System.Linq;

    public static class StringExtensions
    {
        public static Guid[]? SplitGuids(this string? source)
        {
            return source?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(Guid.Parse)
                .ToArray() ?? Array.Empty<Guid>();
        }
    }
}
