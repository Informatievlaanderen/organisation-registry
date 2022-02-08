namespace OrganisationRegistry.Handling
{
    using Authorization;
    using Infrastructure.Authorization;
    using Organisation;

    public static class HandlerExtensionMethods
    {
        public static Handler WithVlimpersParentChildPolicy(
            this Handler source,
            OrganisationId parentOrganisationId,
            OrganisationId childOrganisationId)
        {
            return source.WithPolicy(new VlimpersParentChildPolicy(childOrganisationId));
        }

        public static Handler RequiresAdmin(this Handler source)
        {
            return source.WithPolicy(new AdminOnlyPolicy());
        }

        public static Handler RequiresOneOfRole(this Handler source, params Role[] roles)
        {
            return source.WithPolicy(new RequiresRolesPolicy(roles));
        }
    }
}
