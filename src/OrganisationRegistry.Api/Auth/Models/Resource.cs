namespace OrganisationRegistry.Api.Auth.Models;

using System;
using System.Collections.Generic;
using System.Linq;

using GlobalResource = Resource<GlobalResources>;
using OrganisationResource = Resource<OrganisationResources>;
using BodyResource = Resource<BodyResources>;
public readonly struct Resource<TResource> where TResource : struct, Enum
{
    public TResource Name { get; }
    public CrudOperation Operations { get; }

    private Resource(TResource name, CrudOperation operations)
    {
        Name = name;
        Operations = operations;
    }

    public static Resource<TResource> Create(TResource resource, CrudOperation operations)
        => new(resource, operations);

    public IEnumerable<string> ToPermissionStrings()
    {
        var resource = Name.PermissionName;
        var operations = Operations;

        return Enum.GetValues<CrudOperation>()
            .Where(op => op != CrudOperation.None && operations.HasFlag(op))
            .Select(op => $"{resource}:{op.ToString().ToLowerInvariant()}");
    }
}

