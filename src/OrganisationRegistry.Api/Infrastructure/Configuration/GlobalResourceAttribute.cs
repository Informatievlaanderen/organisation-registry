namespace OrganisationRegistry.Api.Infrastructure.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using Auth.Models;

public class GlobalResourceAttribute : Attribute
{
    public readonly ResourceDefinition ResourceDefinition;
    public readonly CrudOperation Operation;

    public GlobalResourceAttribute(ResourceDefinition resourceDefinition, CrudOperation operation)
    {
        ResourceDefinition = resourceDefinition;
        Operation = operation;
    }

    public IList<CrudOperation> CrudOperations
    {
        get
        {
            return Enum.GetValues<CrudOperation>()
                .Where(operation =>
                    operation != CrudOperation.None &&
                    Operation.HasFlag(operation))
                .ToList();
        }
    }
}
