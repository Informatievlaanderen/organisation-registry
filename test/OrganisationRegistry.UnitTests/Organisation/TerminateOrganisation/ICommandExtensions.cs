namespace OrganisationRegistry.UnitTests.Organisation.TerminateOrganisation
{
    using System;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Bus;
    using OrganisationRegistry.Infrastructure.Commands;

    public static class ICommandExtensions
    {
        public static T WithUserRole<T>(this T source, Role role) where T : ICommand
        {
            var user = (source.User ?? new User(string.Empty, string.Empty, string.Empty, string.Empty,
                Array.Empty<Role>(), Array.Empty<string>()));
            user.Roles = new[] {role};
            source.User = user;

            return source;
        }
    }
}
