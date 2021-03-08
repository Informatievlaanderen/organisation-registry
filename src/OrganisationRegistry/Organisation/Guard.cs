namespace OrganisationRegistry.Organisation
{
    using Infrastructure.Authorization;

    public static class Guard
    {
        public static void RequiresRole(IUser user, Role role)
        {
            if (!user.IsInRole(role))
                throw new InsuffientsRights();
        }
    }
}
