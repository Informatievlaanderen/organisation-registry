namespace OrganisationRegistry.Api.Backoffice.Organisation;

using System;
using OrganisationRegistry.Handling.Authorization;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;

/// <summary>
/// Per-organisation capability summary for the current caller, returned on
/// organisation list items so the UI can gate tabs and actions without
/// re-deriving authorization.
///
/// Each feature flag answers "may this user manage <em>at least</em> this feature
/// on this organisation?". For resource-typed permissions (capacities, formal
/// frameworks, classifications, keys) the check uses an empty resource context,
/// which yields vacuous truth: the flag is true when the user holds the
/// permission for this organisation scope, regardless of the specific resource
/// subset they are allowed to touch. Row-level rights remain enforced by the
/// individual sub-resource endpoints.
/// </summary>
public class OrganisationPermissions
{
    public bool CanEdit { get; }
    public bool CanDelete { get; }
    public bool CanManageChildren { get; }
    public bool CanManageContacts { get; }
    public bool CanManageLocations { get; }
    public bool CanManageBuildings { get; }
    public bool CanManageFunctions { get; }
    public bool CanManageCapacities { get; }
    public bool CanManageNames { get; }
    public bool CanManageClassifications { get; }
    public bool CanManageFormalFrameworks { get; }
    public bool CanManageKeys { get; }
    public bool CanManageRelations { get; }
    public bool CanManageLabels { get; }
    public bool CanManageRegulations { get; }
    public bool CanManageKbo { get; }
    public bool CanManageVlimpers { get; }

    private OrganisationPermissions(
        bool canEdit,
        bool canDelete,
        bool canManageChildren,
        bool canManageContacts,
        bool canManageLocations,
        bool canManageBuildings,
        bool canManageFunctions,
        bool canManageCapacities,
        bool canManageNames,
        bool canManageClassifications,
        bool canManageFormalFrameworks,
        bool canManageKeys,
        bool canManageRelations,
        bool canManageLabels,
        bool canManageRegulations,
        bool canManageKbo,
        bool canManageVlimpers)
    {
        CanEdit = canEdit;
        CanDelete = canDelete;
        CanManageChildren = canManageChildren;
        CanManageContacts = canManageContacts;
        CanManageLocations = canManageLocations;
        CanManageBuildings = canManageBuildings;
        CanManageFunctions = canManageFunctions;
        CanManageCapacities = canManageCapacities;
        CanManageNames = canManageNames;
        CanManageClassifications = canManageClassifications;
        CanManageFormalFrameworks = canManageFormalFrameworks;
        CanManageKeys = canManageKeys;
        CanManageRelations = canManageRelations;
        CanManageLabels = canManageLabels;
        CanManageRegulations = canManageRegulations;
        CanManageKbo = canManageKbo;
        CanManageVlimpers = canManageVlimpers;
    }

    public static OrganisationPermissions For(IUser user, string ovoNumber, bool isUnderVlimpersManagement)
    {
        var userContext = new UserContext(user);
        var organisationContext = new OrganisationContext(ovoNumber);

        bool Satisfies(Permission permission, params IRestrictionContext[] contexts)
            => user.IsSatisfiedFor(permission, contexts);

        var canEditOrganisation =
            new BeheerderForOrganisationButNotUnderVlimpersManagementPolicy(isUnderVlimpersManagement, ovoNumber)
                .Check(user)
                .IsSuccessful;

        return new OrganisationPermissions(
            canEdit: canEditOrganisation,
            canDelete: false,
            canManageChildren: Satisfies(Permission.CanManageChildren, userContext, organisationContext),
            canManageContacts: Satisfies(Permission.CanManageContacts, userContext, organisationContext),
            canManageLocations: Satisfies(Permission.CanManageLocations, userContext, organisationContext),
            canManageBuildings: Satisfies(Permission.CanManageBuildings, userContext, organisationContext),
            canManageFunctions: Satisfies(Permission.CanManageFunctions, userContext, organisationContext),
            canManageCapacities: Satisfies(
                Permission.CanManageCapacities,
                userContext,
                organisationContext,
                new CapacityContext(Array.Empty<Guid>())),
            canManageNames: canEditOrganisation,
            canManageClassifications: Satisfies(
                Permission.CanManageOrganisationClassifications,
                userContext,
                organisationContext,
                new ClassificationTypeContext(Array.Empty<Guid>())),
            canManageFormalFrameworks: Satisfies(
                Permission.CanManageFormalFrameworks,
                userContext,
                organisationContext,
                new FormalFrameworkContext(Array.Empty<Guid>())),
            canManageKeys: Satisfies(
                Permission.CanManageKeys,
                userContext,
                new KeyContext(isUnderVlimpersManagement, Array.Empty<Guid>())),
            canManageRelations: Satisfies(Permission.CanManageRelations, userContext, organisationContext),
            canManageLabels: Satisfies(
                Permission.CanManageLabels,
                userContext,
                organisationContext,
                new LabelContext(isUnderVlimpersManagement, Array.Empty<Guid>())),
            canManageRegulations: Satisfies(Permission.CanManageRegulations),
            canManageKbo: Satisfies(Permission.CanManageKbo),
            canManageVlimpers: Satisfies(Permission.CanManageVlimpers));
    }
}
