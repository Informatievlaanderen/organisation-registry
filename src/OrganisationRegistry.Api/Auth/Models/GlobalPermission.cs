namespace OrganisationRegistry.Api.Auth.Models;

using System;
using System.Collections.Generic;
using System.Linq;

[Flags]
public enum CrudOperation
{
    None = 0,
    Read   = 1 << 0, // 0001
    Create = 1 << 1, // 0010
    Write  = 1 << 2, // 0100
    Delete = 1 << 3, // 1000
}

public readonly struct GlobalPermission(ResourceDefinition resource, CrudOperation operations)
{
    public ResourceDefinition Resource { get; } = resource;
    public CrudOperation Operations { get; } = operations;

    public IEnumerable<string> ToPermissionStrings()
    {
        var resource = Resource.PermissionName;
        var operations = Operations;

        return Enum.GetValues<CrudOperation>()
            .Where(op => op != CrudOperation.None && operations.HasFlag(op))
            .Select(op => $"{resource}:{op.ToString().ToLowerInvariant()}");
    }
}
