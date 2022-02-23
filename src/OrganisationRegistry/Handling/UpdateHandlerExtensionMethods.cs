namespace OrganisationRegistry.Handling
{
    using System.Linq;
    using Authorization;
    using Configuration;
    using Infrastructure.Authorization;
    using Organisation;
    using Organisation.Commands;

    public static class UpdateHandlerExtensionMethods
    {
        public static UpdateHandler<Organisation> WithVlimpersPolicy(this UpdateHandler<Organisation> source)
        {
            return source.WithPolicy(organisation =>
                new VlimpersPolicy(
                    organisation.State.UnderVlimpersManagement,
                    organisation.State.OvoNumber));
        }

        public static UpdateHandler<Organisation> WithVlimpersOnlyPolicy(this UpdateHandler<Organisation> source)
        {
            return source.WithPolicy(organisation =>
                new VlimpersOnlyPolicy(
                    organisation.State.UnderVlimpersManagement));
        }

        public static UpdateHandler<Organisation> WithLabelPolicy(this UpdateHandler<Organisation> source,
            IOrganisationRegistryConfiguration configuration, AddOrganisationLabel message)
        {
            return source.WithPolicy(organisation =>
                new LabelPolicy(
                    organisation.State.OvoNumber,
                    organisation.State.UnderVlimpersManagement,
                    configuration,
                    message.LabelTypeId));
        }

        public static UpdateHandler<Organisation> WithLabelPolicy(this UpdateHandler<Organisation> source,
            IOrganisationRegistryConfiguration configuration, UpdateOrganisationLabel message)
        {
            return source.WithPolicy(organisation =>
                new LabelPolicy(
                    organisation.State.OvoNumber,
                    organisation.State.UnderVlimpersManagement,
                    configuration,
                    message.LabelTypeId,
                    organisation.State.OrganisationLabels
                        .Single(x => x.OrganisationLabelId == message.OrganisationLabelId).LabelTypeId));
        }

        public static UpdateHandler<Organisation> WithKeyPolicy(this UpdateHandler<Organisation> source,
            IOrganisationRegistryConfiguration configuration, AddOrganisationKey message)
        {
            return source.WithPolicy(organisation =>
                new KeyPolicy(
                    organisation.State.OvoNumber,
                    organisation.State.UnderVlimpersManagement,
                    configuration, message.KeyTypeId));
        }

        public static UpdateHandler<Organisation> WithKeyPolicy(this UpdateHandler<Organisation> source,
            IOrganisationRegistryConfiguration configuration, UpdateOrganisationKey message)
        {
            return source.WithPolicy(organisation =>
                new KeyPolicy(
                    organisation.State.OvoNumber,
                    organisation.State.UnderVlimpersManagement,
                    configuration, message.KeyTypeId));
        }

        public static UpdateHandler<Organisation> RequiresBeheerderForOrganisation(
            this UpdateHandler<Organisation> source)
        {
            return source.WithPolicy(organisation =>
                new BeheerderForOrganisationPolicy(
                    organisation.State.UnderVlimpersManagement,
                    organisation.State.OvoNumber));
        }

        public static UpdateHandler<Organisation> RequiresAdmin(this UpdateHandler<Organisation> source)
        {
            return source.WithPolicy(_ => new AdminOnlyPolicy());
        }

        public static UpdateHandler<Organisation> RequiresOneOfRole(this UpdateHandler<Organisation> source, params Role[] roles)
        {
            return source.WithPolicy(_ => new RequiresRolesPolicy(roles));
        }
    }
}
