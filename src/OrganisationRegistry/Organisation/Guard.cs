namespace OrganisationRegistry.Organisation
{
    using System.Linq;
    using Infrastructure.Authorization;

    public static class Guard
    {
        public static void RequiresRole(IUser user, Role role)
        {
            if (!user.IsInRole(role))
                throw new InsuffientsRights();
        }

        public static void RequiresOneOfRoles(IUser user, params Role[] roles)
        {
            if (roles.Any(user.IsInRole))
                return;

            throw new InsuffientsRights();
        }
    }
}
