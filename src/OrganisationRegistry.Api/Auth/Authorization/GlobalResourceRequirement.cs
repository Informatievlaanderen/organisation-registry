namespace OrganisationRegistry.Api.Auth.Authorization;

using Microsoft.AspNetCore.Authorization;
using Models;

public sealed class GlobalResourceRequirement(GlobalResources resource, CrudOperation operations)
    : IAuthorizationRequirement
{
    public GlobalResources Resource { get; } = resource;
    public CrudOperation Operations { get; } = operations;
}
