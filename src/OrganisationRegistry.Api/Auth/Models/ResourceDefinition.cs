namespace OrganisationRegistry.Api.Auth.Models;

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

public enum GlobalResources
{
    [Description("org.organisations")] OrgOrganisations,
    [Description("body.info")] BodyInfo,
    [Description("reports")] Reports,
    [Description("ref.parameters")] RefParameters,
    [Description("imports")] Imports,
    [Description("delegations")] Delegations,
}

public enum OrganisationResources
{
    [Description("canEdit")] CanEdit,
    [Description("canDelete")] CanDelete,
    [Description("canManageChildren")] CanManageChildren,
    [Description("canManageContacts")] CanManageContacts,
    [Description("canViewFunctions")] CanViewFunctions,
    [Description("canManageFunctions")] CanManageFunctions,
    [Description("canViewCapacities")] CanViewCapacities,
    [Description("canManageCapacities")] CanManageCapacities,
    [Description("canManageLocations")] CanManageLocations,
    [Description("canManageBuildings")] CanManageBuildings,
    [Description("canManageLabels")] CanManageLabels,
    [Description("canManageClassifications")] CanManageClassifications,
    [Description("canManageFormalFrameworks")] CanManageFormalFrameworks,
    [Description("canManageKeys")] CanManageKeys,
    [Description("canManageRegulations")] CanManageRegulations,
    [Description("canManageBodies")] CanManageBodies,
    [Description("canManageRelations")] CanManageRelations,
    [Description("canViewKbo")] CanViewKbo,
    [Description("canManageKbo")] CanManageKbo,
    [Description("canViewVlimpers")] CanViewVlimpers,
    [Description("canManageVlimpers")] CanManageVlimpers
}

public enum BodyResources
{
    [Description("canEdit")] CanEdit,
    [Description("canDelete")] CanDelete,
    [Description("canManageContacts")] CanManageContacts,
    [Description("canManageSeats")] CanManageSeats,
    [Description("canManageMandates")] CanManageMandates,
    [Description("canManageLifecycles")] CanManageLifecycles,
    [Description("canManageOrganisations")] CanManageOrganisations,
    [Description("canManageFormalFrameworks")] CanManageFormalFrameworks,
    [Description("canManageMep")] CanManageMep,
    [Description("canManageClassifications")] CanManageClassifications
}

public static class ResourceExtensions
{
    private static readonly ConcurrentDictionary<Enum, string> _cache = new();

    extension<TEnum>(TEnum resource) where TEnum : struct, Enum
    {
        public string PermissionName => _cache.GetOrAdd(resource, static e =>
        {
            var member = e.GetType().GetMember(e.ToString())[0];
            var description = member.GetCustomAttribute<DescriptionAttribute>();
            return description?.Description ?? e.ToString();
        });
    }
}
