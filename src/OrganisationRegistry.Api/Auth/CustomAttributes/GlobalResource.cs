namespace OrganisationRegistry.Api.Auth.CustomAttributes;

using System;
using Models;

/// <summary>
/// Marks an endpoint as requiring access to a given <see cref="ResourceDefinition"/>.
/// This is the link between an API endpoint and the resource that
/// <see cref="RolePermissions"/> (the source of truth for role -> permission
/// mapping, via <see cref="GlobalPermission"/>) grants or withholds per role.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class GlobalResource(ResourceDefinition resource, CrudOperation operations) : Attribute
{
    public ResourceDefinition Resource { get; } = resource;
    public CrudOperation Operations { get; } = operations;
}
