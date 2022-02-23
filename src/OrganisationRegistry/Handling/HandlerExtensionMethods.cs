namespace OrganisationRegistry.Handling
{
    using System;
    using Authorization;
    using Configuration;
    using Infrastructure.Authorization;
    using Organisation;

    public static class HandlerExtensionMethods
    {
        public static Handler WithVlimpersPolicy(this Handler source, Organisation organisation)
            => source.WithPolicy(
                new VlimpersPolicy(
                    organisation.State.UnderVlimpersManagement,
                    organisation.State.OvoNumber));

        public static Handler WithVlimpersOnlyPolicy(this Handler source, Organisation organisation)
            => source.WithPolicy(
                new VlimpersOnlyPolicy(
                    organisation.State.UnderVlimpersManagement));

        public static Handler WithLabelPolicy(
            this Handler source,
            Organisation organisation,
            IOrganisationRegistryConfiguration configuration,
            params Guid[] labelTypeIds)
            => source.WithPolicy(
                new LabelPolicy(
                    organisation.State.OvoNumber,
                    organisation.State.UnderVlimpersManagement,
                    configuration,
                    labelTypeIds));

        public static Handler WithKeyPolicy(
            this Handler source,
            Organisation organisation,
            Guid keyTypeId,
            IOrganisationRegistryConfiguration configuration)
            => source.WithPolicy(
                new KeyPolicy(
                    organisation.State.OvoNumber,
                    organisation.State.UnderVlimpersManagement,
                    configuration,
                    keyTypeId));

        public static Handler RequiresBeheerderForOrganisation(this Handler source, Organisation organisation)
            => source.WithPolicy(
                new BeheerderForOrganisationPolicy(
                    organisation.State.UnderVlimpersManagement,
                    organisation.State.OvoNumber));

        public static Handler RequiresAdmin(this Handler source)
            => source.WithPolicy(new AdminOnlyPolicy());

        public static Handler RequiresOneOfRole(this Handler source, params Role[] roles)
            => source.WithPolicy(new RequiresRolesPolicy(roles));
    }
}
