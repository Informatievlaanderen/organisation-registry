namespace OrganisationRegistry
{
    using System;
    using System.Linq;

    public static class StringExtensions
    {
        public static Guid[] SplitGuids(this string? source)
            => source?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(Guid.Parse)
                .ToArray() ?? Array.Empty<Guid>();

        public static bool IsNotEmptyOrWhiteSpace(this string source)
            => source.Trim().Any();
    }
}
