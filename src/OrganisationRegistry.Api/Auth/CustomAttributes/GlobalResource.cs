namespace OrganisationRegistry.Api.Auth.CustomAttributes;

using System;
using System.Collections.Generic;
using Auth.Authorization;
using Microsoft.AspNetCore.Authorization;
using Models;

[AttributeUsage(AttributeTargets.Method)]
public sealed class GlobalResourceAttribute(GlobalResources resource, CrudOperation operations)
    : AuthorizeAttribute, IAuthorizationRequirementData
{
    public GlobalResources Resource { get; } = resource;
    public CrudOperation Operations { get; } = operations;

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return new GlobalResourceRequirement(Resource, Operations);
    }
}
