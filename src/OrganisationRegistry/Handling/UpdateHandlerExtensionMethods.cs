namespace OrganisationRegistry.Handling;

using System.Linq;
using Authorization;
using Body;
using Infrastructure.Authorization;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using Organisation;

public static class UpdateHandlerExtensionMethods
{
    public static UpdateHandler<Organisation> WithVlimpersPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(
            organisation => new VlimpersPolicy(
                organisation.State.UnderVlimpersManagement,
                organisation.State.OvoNumber));

    public static UpdateHandler<Organisation> WithBeheerderForOrganisationPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(
            organisation => new BeheerderForOrganisationRegardlessOfVlimpersPolicy(
                organisation.State.OvoNumber));

    public static UpdateHandler<Organisation> WithVlimpersOnlyPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(organisation => new VlimpersOnlyPolicy(organisation.State.UnderVlimpersManagement));

    public static UpdateHandler<Organisation> WithLabelPolicy(
        this UpdateHandler<Organisation> source,
        IOrganisationRegistryConfiguration configuration,
        AddOrganisationLabel message)
        => source.WithPolicy(
            organisation => LabelPolicy.ForCreate(
                organisation.State.OvoNumber,
                organisation.State.UnderVlimpersManagement,
                configuration,
                message.LabelTypeId));

    public static UpdateHandler<Organisation> WithLabelPolicy(
        this UpdateHandler<Organisation> source,
        IOrganisationRegistryConfiguration configuration,
        UpdateOrganisationLabel message)
        => source.WithPolicy(
            organisation => LabelPolicy.ForUpdate(
                organisation.State.OvoNumber,
                organisation.State.UnderVlimpersManagement,
                configuration,
                organisation.State.OrganisationLabels
                    .Single(x => x.OrganisationLabelId == message.OrganisationLabelId).LabelTypeId,
                message.LabelTypeId));

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

    public static UpdateHandler<Organisation> WithCapacityPolicy(
        this UpdateHandler<Organisation> source,
        IOrganisationRegistryConfiguration configuration,
        AddOrganisationCapacity message)
        => source.WithPolicy(
            organisation =>
                new CapacityPolicy(
                    organisation.State.OvoNumber,
                    configuration,
                    message.CapacityId));

    public static UpdateHandler<Organisation> WithCapacityPolicy(
        this UpdateHandler<Organisation> source,
        IOrganisationRegistryConfiguration configuration,
        UpdateOrganisationCapacity message)
        => source.WithPolicy(
            organisation =>
                new CapacityPolicy(
                    organisation.State.OvoNumber,
                    configuration,
                    message.CapacityId));

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

    public static UpdateHandler<TAggregate> RequiresOneOfRole<TAggregate>(
        this UpdateHandler<TAggregate> source,
        params Role[] roles)
        where TAggregate : AggregateRoot
        => source.WithPolicy(_ => new RequiresRolesPolicy(roles));

    public static UpdateHandler<Body> WithEditBodyPolicy(this UpdateHandler<Body> source)
        => source.WithPolicy(body => new EditBodyPolicy(body.Id));

    public static UpdateHandler<Body> WithEditDelegationPolicy(this UpdateHandler<Body> source, OrganisationId organisationId)
        => source.WithPolicy(body => new EditDelegationPolicy(organisationId, body.Id));
}
