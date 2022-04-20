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
            => source.WithPolicy(
                organisation => new VlimpersPolicy(
                    organisation.State.UnderVlimpersManagement,
                    organisation.State.OvoNumber));

        public static UpdateHandler<Organisation> WithVlimpersOnlyPolicy(this UpdateHandler<Organisation> source)
            => source.WithPolicy(organisation => new VlimpersOnlyPolicy(organisation.State.UnderVlimpersManagement));

        public static UpdateHandler<Organisation> WithLabelPolicy(
            this UpdateHandler<Organisation> source,
            IOrganisationRegistryConfiguration configuration,
            AddOrganisationLabel message)
            => source.WithPolicy(
                organisation => new LabelPolicy(
                    organisation.State.OvoNumber,
                    organisation.State.UnderVlimpersManagement,
                    configuration,
                    message.LabelTypeId));

        public static UpdateHandler<Organisation> WithLabelPolicy(
            this UpdateHandler<Organisation> source,
            IOrganisationRegistryConfiguration configuration,
            UpdateOrganisationLabel message)
            => source.WithPolicy(
                organisation => new LabelPolicy(
                    organisation.State.OvoNumber,
                    organisation.State.UnderVlimpersManagement,
                    configuration,
                    message.LabelTypeId,
                    organisation.State.OrganisationLabels
                        .Single(x => x.OrganisationLabelId == message.OrganisationLabelId).LabelTypeId));

        public static UpdateHandler<Organisation> WithKeyPolicy(
            this UpdateHandler<Organisation> source,
            IOrganisationRegistryConfiguration configuration,
            AddOrganisationKey message)
            => source.WithPolicy(
                organisation => new KeyPolicy(
                    organisation.State.OvoNumber,
                    configuration,
                    message.KeyTypeId));

        public static UpdateHandler<Organisation> WithKeyPolicy(
            this UpdateHandler<Organisation> source,
            IOrganisationRegistryConfiguration configuration,
            UpdateOrganisationKey message)
            => source.WithPolicy(
                organisation => new KeyPolicy(
                    organisation.State.OvoNumber,
                    configuration,
                    message.KeyTypeId));

        public static UpdateHandler<Organisation> WithOrganisationClassificationTypePolicy(
            this UpdateHandler<Organisation> source,
            IOrganisationRegistryConfiguration configuration,
            AddOrganisationOrganisationClassification message)
            => source.WithPolicy(
                organisation =>
                    new OrganisationClassificationTypePolicy(
                        organisation.State.OvoNumber,
                        configuration,
                        message.OrganisationClassificationTypeId));

        public static UpdateHandler<Organisation> WithOrganisationClassificationTypePolicy(
            this UpdateHandler<Organisation> source,
            IOrganisationRegistryConfiguration configuration,
            UpdateOrganisationOrganisationClassification message)
            => source.WithPolicy(
                organisation =>
                    new OrganisationClassificationTypePolicy(
                        organisation.State.OvoNumber,
                        configuration,
                        message.OrganisationClassificationTypeId));

        public static UpdateHandler<Organisation> RequiresBeheerderForOrganisationButNotUnderVlimpersManagement(
            this UpdateHandler<Organisation> source)
            => source.WithPolicy(
                organisation => new BeheerderForOrganisationButNotUnderVlimpersManagementPolicy(
                    organisation.State.UnderVlimpersManagement,
                    organisation.State.OvoNumber));

        public static UpdateHandler<Organisation> RequiresBeheerderForOrganisationRegardlessOfVlimpers(
            this UpdateHandler<Organisation> source)
            => source.WithPolicy(
                organisation => new BeheerderForOrganisationRegardlessOfVlimpersPolicy(organisation.State.OvoNumber));

        public static UpdateHandler<Organisation> RequiresAdmin(this UpdateHandler<Organisation> source)
            => source.WithPolicy(_ => new AdminOnlyPolicy());

        public static UpdateHandler<Organisation> RequiresOneOfRole(
            this UpdateHandler<Organisation> source,
            params Role[] roles)
            => source.WithPolicy(_ => new RequiresRolesPolicy(roles));
    }
}
