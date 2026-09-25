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
    public static UpdateHandler<Organisation> WithChildPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(
            organisation => new ChildPolicy(
                organisation.State.OvoNumber,
                organisation.State.UnderVlimpersManagement));

    /// <summary>
    /// Authorization for managing an already-existing organisation's parent
    /// coupling (<see cref="Permission.CanManageParent"/>), evaluated against
    /// the organisation being reparented (the aggregate loaded by
    /// <see cref="UpdateHandler{TAggregate}.For"/>, i.e. the command's own
    /// <c>Id</c>) — not the target parent organisation.
    /// </summary>
    public static UpdateHandler<Organisation> WithParentPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(
            organisation => new OrganisationPolicy(
                Permission.CanManageParent,
                organisation.State.OvoNumber,
                organisation.State.UnderVlimpersManagement));

    public static UpdateHandler<Organisation> WithContactPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(_ => new ContactPolicy());

    public static UpdateHandler<Organisation> WithKboPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(_ => new KboPolicy());

    public static UpdateHandler<Organisation> WithVlimpersManagementPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(_ => new VlimpersManagementPolicy());

    public static UpdateHandler<Organisation> WithFunctionPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(organisation => new FunctionPolicy(organisation.State.OvoNumber));

    public static UpdateHandler<Organisation> WithBuildingPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(organisation => new BuildingPolicy(organisation.State.OvoNumber));

    public static UpdateHandler<Organisation> WithRelationPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(organisation => new RelationPolicy(organisation.State.OvoNumber));

    public static UpdateHandler<Organisation> WithLocationPolicy(this UpdateHandler<Organisation> source)
        => source.WithPolicy(organisation => new LocationPolicy(organisation.State.OvoNumber));

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

    /// <summary>
    /// Authorization for the general organisation info fields via the top-level
    /// endpoint (<see cref="Permission.CanManageOrganisation"/>): AlgemeenBeheerder/
    /// Developer only. VlimpersBeheerder and DecentraalBeheerder must use the split
    /// endpoints/policies instead (<see cref="RequiresBeheerderForOrganisationLimitedToVlimpers"/>
    /// and <see cref="RequiresBeheerderForOrganisationNotLimitedToVlimpers"/>).
    /// </summary>
    public static UpdateHandler<Organisation> RequiresBeheerderForOrganisationButNotUnderVlimpersManagement(
        this UpdateHandler<Organisation> source)
        => source.WithPolicy(
            organisation => new OrganisationPolicy(
                Permission.CanManageOrganisation,
                organisation.State.OvoNumber,
                organisation.State.UnderVlimpersManagement));

    /// <summary>
    /// Authorization for the four fields whose management is reserved to
    /// Vlimpers (<see cref="Permission.CanManageOrganisationInfoLimitedToVlimpers"/>):
    /// AlgemeenBeheerder/Developer unrestricted, VlimpersBeheerder only for
    /// organisations currently under Vlimpers management. Unlike
    /// <see cref="RequiresBeheerderForOrganisationButNotUnderVlimpersManagement"/>,
    /// DecentraalBeheerder never passes this check, regardless of the
    /// organisation's Vlimpers-management status or ownership.
    /// </summary>
    public static UpdateHandler<Organisation> RequiresBeheerderForOrganisationLimitedToVlimpers(
        this UpdateHandler<Organisation> source)
        => source.WithPolicy(
            organisation => new OrganisationPolicy(
                Permission.CanManageOrganisationInfoLimitedToVlimpers,
                organisation.State.OvoNumber,
                organisation.State.UnderVlimpersManagement));

    /// <summary>
    /// Authorization for the organisation fields <em>not</em> reserved to
    /// Vlimpers (<see cref="Permission.CanManageOrganisationInfoNotLimitedToVlimpers"/>):
    /// AlgemeenBeheerder/Developer unrestricted, DecentraalBeheerder for their
    /// own organisation regardless of Vlimpers-management status.
    /// VlimpersBeheerder never passes this check.
    /// </summary>
    public static UpdateHandler<Organisation> RequiresBeheerderForOrganisationNotLimitedToVlimpers(
        this UpdateHandler<Organisation> source)
        => source.WithPolicy(
            organisation => new OrganisationPolicy(
                Permission.CanManageOrganisationInfoNotLimitedToVlimpers,
                organisation.State.OvoNumber,
                organisation.State.UnderVlimpersManagement));

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

    public static UpdateHandler<TAggregate> WithPeoplePolicy<TAggregate>(this UpdateHandler<TAggregate> source)
        where TAggregate : AggregateRoot
        => source.WithPolicy(_ => new PeoplePolicy());
}
