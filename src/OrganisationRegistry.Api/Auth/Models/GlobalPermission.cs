namespace OrganisationRegistry.Api.Auth.Models;

using System;
using System.Collections.Generic;
using System.Linq;

// TODO Controleren welke meer of minder belangrijk is (create boven write?)
[Flags]
public enum CrudOperation
{
    None   = 0,
    Read   = 1 << 0,
    Create = 1 << 1,
    Write  = 1 << 2,
    Delete = 1 << 3,
}

public readonly struct GlobalPermission
{
    public string Resource { get; }
    public CrudOperation Operations { get; }

    public GlobalPermission(string resource, CrudOperation operations)
    {
        Resource = resource;
        Operations = operations;
    }

    public IEnumerable<string> ToPermissionStrings()
    {
        var resource = Resource;
        var operations = Operations;

        return Enum.GetValues<CrudOperation>()
            .Where(op => op != CrudOperation.None && operations.HasFlag(op))
            .Select(op => $"{resource}:{op.ToString().ToLowerInvariant()}");
    }
}
