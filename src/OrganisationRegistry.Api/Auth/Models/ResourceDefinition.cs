namespace OrganisationRegistry.Api.Auth.Models;

using System.Collections.Generic;

public enum ResourceDefinition
{
    OrgOrganisations,
    BodyInfo,
    Reports,
    RefParameters,
    Imports,
    Delegations,
}

public static class ResourceDefinitionExtensions
{
    private static readonly Dictionary<ResourceDefinition, string> PermissionNames = new()
    {
        [ResourceDefinition.OrgOrganisations] = "org.organisations",
        [ResourceDefinition.BodyInfo] = "body.info",
        [ResourceDefinition.Reports] = "reports",
        [ResourceDefinition.RefParameters] = "ref.parameters",
        [ResourceDefinition.Imports] = "imports",
        [ResourceDefinition.Delegations] = "delegations",
    };

    extension(ResourceDefinition resource)
    {
        public string PermissionName
            => PermissionNames[resource];
    }
}

