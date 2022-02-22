namespace OrganisationRegistry.UnitTests.Organisation.TerminateOrganisation
{
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;

    public static class ICommandExtensions
    {
        public static T WithUserRole<T>(this T source, Role role) where T : ICommand
        {
            var user = source.User ?? WellknownUsers.Nobody;
            user.Roles = new[] {role};
            source.User = user;

            return source;
        }
    }
}
