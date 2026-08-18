
namespace OrganisationRegistry.Api.Auth.Models;

using System;
using System.Collections.Generic;
using System.Linq;

[Flags]
public enum CrudOperation
{
    None = 0,
    Read = 1 << 0, // 0001
    Create = 1 << 1, // 0010
    Write = 1 << 2, // 0100
    Delete = 1 << 3, // 1000
}

public static class CrudOperationExtensions
{
    public static IEnumerable<CrudOperation> GetOperations(this CrudOperation operations)
        => Enum.GetValues<CrudOperation>()
               .Where(op => op != CrudOperation.None && operations.HasFlag(op));

    public static IEnumerable<string> ToOperationStrings(this CrudOperation operations)
        => operations.GetOperations().Select(op => op.ToString().ToLowerInvariant());
}
