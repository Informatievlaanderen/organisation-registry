namespace OrganisationRegistry.Acl.Impl;

using Internals;
using OrganisationRegistry.Infrastructure.Authorization;
using System;
using System.Collections.Generic;


public static class OrganisationCapabilities
{
    public static readonly Capability CanViewCapacities = new("canViewCapacities", Tags.Organisation);
    public static readonly Capability CanManageChildren = new("canManageChildren", Tags.Organisation);
}

public readonly record struct OrganisationData(
    CrudOperation Ops,
    string SenderOvoCode,
    string TargetOrganisationOvoCode,
    HashSet<string> ChildrenOvoCodes,
    bool IsVlimpersActiveForTargetOrganisation) : IOperationRequest;

// public readonly record struct OrganisationData(
//     CrudOperation Ops,
//     Lazy<IUser> User) : IOperationRequest;

public static class OrganisationRules
{
    public static Result OvoScoping(OrganisationData data)
        => data.TargetOrganisationOvoCode == data.SenderOvoCode
           || data.ChildrenOvoCodes.Contains(data.TargetOrganisationOvoCode)
            ? Result.Success
            : Result.Failed("Target organisation falls outside the acting user's OVO scope.");

    public static Result VlimpersScoping(OrganisationData data)
        => data.IsVlimpersActiveForTargetOrganisation
            ? Result.Success
            : Result.Failed("Vlimpers is not active for the target organisation.");
}

public sealed class OrganisationProvider : IRulesProvider<OrganisationData>
{
    private static readonly Rule<OrganisationData>[] FullAccess = [ Rule.Define<OrganisationData>(
        CrudOperation.Read |
        CrudOperation.Create |
        CrudOperation.Write |
        CrudOperation.Delete
    )];

    private static readonly Rule<OrganisationData>[] ScopedToOwnOvo =
    [
        Rule.Define<OrganisationData>(CrudOperation.Read | CrudOperation.Create | CrudOperation.Write | CrudOperation.Delete),
        OrganisationRules.OvoScoping
    ];

    private static readonly Rule<OrganisationData>[] ReadOnlyIfVlimpersActive =
    [
        Rule.Define<OrganisationData>(CrudOperation.Read),
        OrganisationRules.VlimpersScoping
    ];

    private static readonly Rule<OrganisationData>[] ReadOnly =
    [
        Rule.Define<OrganisationData>(CrudOperation.Read)
    ];

    private static readonly Rule<OrganisationData>[] None = [];

    public Rule<OrganisationData>[] For(Role role, Capability capabilities) => (role, capabilities.Name) switch
    {
        (Role.AlgemeenBeheerder, "canViewCapacities") => FullAccess,
        (Role.DecentraalBeheerder, "canViewCapacities") => ScopedToOwnOvo,
        (Role.VlimpersBeheerder, "canViewCapacities") => ReadOnlyIfVlimpersActive,
        (Role.OrgaanBeheerder, "canViewCapacities") => ReadOnly,
        (Role.OrgaanBeheerder, "GLOB::RESOURCE") => ReadOnly,
        (Role.RegelgevingBeheerder, "canViewCapacities") => FullAccess,
        _ => None,
    };
}
