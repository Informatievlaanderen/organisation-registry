namespace OrganisationRegistry.Handling
{
    using Authorization;
    using Infrastructure.Authorization;
    using Organisation;

    public static class HandlerExtensionMethods
    {
        public static Handler WithVlimpersPolicy(this Handler source, Organisation organisation)
        {
            return source.WithPolicy(
                new VlimpersPolicy(
                    organisation.State.UnderVlimpersManagement,
                    organisation.State.OvoNumber));
        }

        public static Handler RequiresBeheerderForOrganisation(this Handler source, Organisation organisation)
        {
            return source.WithPolicy(
                new BeheerderForOrganisationPolicy(
                    organisation.State.UnderVlimpersManagement,
                    organisation.State.OvoNumber));
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
