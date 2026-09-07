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

    public static UpdateHandler<Organisation> WithContactPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(_ => new ContactPolicy());

    public static UpdateHandler<Organisation> WithFunctionPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(organisation => new FunctionPolicy(organisation.State.OvoNumber));

    public static UpdateHandler<Organisation> WithBuildingPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(organisation => new BuildingPolicy(organisation.State.OvoNumber));

    public static UpdateHandler<Organisation> WithRelationPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(organisation => new RelationPolicy(organisation.State.OvoNumber));

    public static UpdateHandler<Organisation> WithLocationPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(organisation => new LocationPolicy(organisation.State.OvoNumber));

    public static UpdateHandler<Organisation> WithVlimpersOnlyPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(organisation => new VlimpersOnlyPolicy(organisation.State.UnderVlimpersManagement));

    public static UpdateHandler<Organisation> WithLabelPolicy(
        this UpdateHandler<Organisation> source,
        AddOrganisationLabel message)
        => source.WithPolicy(
            organisation => LabelPolicy.ForCreate(
                organisation.State.OvoNumber,
                organisation.State.UnderVlimpersManagement,
                message.LabelTypeId));

    public static UpdateHandler<Organisation> WithLabelPolicy(
        this UpdateHandler<Organisation> source,
        UpdateOrganisationLabel message)
        => source.WithPolicy(
            organisation => LabelPolicy.ForUpdate(
                organisation.State.OvoNumber,
                organisation.State.UnderVlimpersManagement,
                organisation.State.OrganisationLabels
                    .Single(x => x.OrganisationLabelId == message.OrganisationLabelId).LabelTypeId,
                message.LabelTypeId));

    public static UpdateHandler<Organisation> WithKeyPolicy(
        this UpdateHandler<Organisation> source,
        AddOrganisationKey message)
        => source.WithPolicy(
            organisation => new KeyPolicy(
                organisation.State.UnderVlimpersManagement,
                message.KeyTypeId));

    public static UpdateHandler<Organisation> WithKeyPolicy(
        this UpdateHandler<Organisation> source,
        UpdateOrganisationKey message)
        => source.WithPolicy(
            organisation => new KeyPolicy(
                organisation.State.UnderVlimpersManagement,
                message.KeyTypeId));

    public static UpdateHandler<Organisation> WithOrganisationClassificationTypePolicy(
        this UpdateHandler<Organisation> source,
        AddOrganisationOrganisationClassification message)
        => source.WithPolicy(
            organisation =>
                new OrganisationClassificationTypePolicy(
                    organisation.State.OvoNumber,
                    message.OrganisationClassificationTypeId));

    public static UpdateHandler<Organisation> WithOrganisationClassificationTypePolicy(
        this UpdateHandler<Organisation> source,
        UpdateOrganisationOrganisationClassification message)
        => source.WithPolicy(
            organisation =>
                new OrganisationClassificationTypePolicy(
                    organisation.State.OvoNumber,
                    message.OrganisationClassificationTypeId));

    public static UpdateHandler<Organisation> WithCapacityPolicy(
        this UpdateHandler<Organisation> source,
        AddOrganisationCapacity message)
        => source.WithPolicy(
            organisation =>
                new CapacityPolicy(
                    organisation.State.OvoNumber,
                    message.CapacityId));

    public static UpdateHandler<Organisation> WithCapacityPolicy(
        this UpdateHandler<Organisation> source,
        UpdateOrganisationCapacity message)
        => source.WithPolicy(
            organisation =>
                new CapacityPolicy(
                    organisation.State.OvoNumber,
                    message.CapacityId));

    public static UpdateHandler<Organisation> WithCapacityPolicy(
        this UpdateHandler<Organisation> source,
        RemoveOrganisationCapacity message)
        => source.WithPolicy(
            organisation =>
                new CapacityPolicy(
                    organisation.State.OvoNumber,
                    organisation.State.OrganisationCapacities
                        .Where(capacity => capacity.OrganisationCapacityId == message.OrganisationCapacityId)
                        .Select(capacity => capacity.CapacityId)
                        .FirstOrDefault()));

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

    public static UpdateHandler<TAggregate> RequiresPermission<TAggregate>(
        this UpdateHandler<TAggregate> source,
        Permission permission)
        where TAggregate : AggregateRoot
        => source.WithPolicy(_ => new RequiresPermissionPolicy(permission));

    public static UpdateHandler<Body> WithBodyPolicy(this UpdateHandler<Body> source, Permission permission)
        => source.WithPolicy(body => new BodyPolicy(permission, body.Id));

    public static UpdateHandler<Body> WithEditDelegationPolicy(this UpdateHandler<Body> source, OrganisationId organisationId)
        => source.WithPolicy(body => new EditDelegationPolicy(organisationId, body.Id));
}
