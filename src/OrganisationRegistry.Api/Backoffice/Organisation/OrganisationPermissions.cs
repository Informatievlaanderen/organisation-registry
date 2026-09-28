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
    /// <summary>
    /// Whether the caller may edit at least one organisation-info field
    /// (general <c>PUT /organisations/{id}</c>, or either of the split
    /// <c>limitedtovlimpers</c> / <c>notlimitedtovlimpers</c> endpoints).
    /// True when the caller holds any of <see cref="Permission.CanManageOrganisation"/>
    /// (AlgemeenBeheerder/Developer), <see cref="Permission.CanManageOrganisationInfoLimitedToVlimpers"/>
    /// (VlimpersBeheerder for organisations under Vlimpers management), or
    /// <see cref="Permission.CanManageOrganisationInfoNotLimitedToVlimpers"/>
    /// (DecentraalBeheerder for their own organisation).
    /// </summary>
    public bool CanEdit { get; }

    /// <summary>
    /// Whether the caller may terminate this organisation (PUT .../terminate).
    /// Unrestricted-only (<see cref="Permission.CanTerminateOrganisation"/>): unlike
    /// <see cref="CanEdit"/>, VlimpersBeheerder and DecentraalBeheerder never hold
    /// this permission, even for their own/Vlimpers-managed organisations.
    /// </summary>
    public bool CanDelete { get; }
    public bool CanManageChildren { get; }
    public bool CanManageContacts { get; }
    public bool CanManageLocations { get; }
    public bool CanManageBuildings { get; }
    public bool CanManageFunctions { get; }
    public bool CanManageCapacities { get; }

    /// <summary>
    /// Whether the caller may view (but not necessarily manage) this
    /// organisation's Functies list. True when the caller holds either
    /// <see cref="Permission.CanManageFunctions"/> (manage implies view) or
    /// the standalone read-only <see cref="Permission.CanViewOrganisationFunctions"/>
    /// (e.g. <see cref="Role.VoMedewerker"/>).
    /// </summary>
    public bool CanViewFunctions { get; }

    /// <summary>
    /// Whether the caller may view (but not necessarily manage) this
    /// organisation's Hoedanigheden list. True when the caller holds either
    /// <see cref="Permission.CanManageCapacities"/> (manage implies view) or
    /// the standalone read-only <see cref="Permission.CanViewOrganisationCapacities"/>
    /// (e.g. <see cref="Role.VoMedewerker"/>).
    /// </summary>
    public bool CanViewCapacities { get; }
    public bool CanManageNames { get; }
    public bool CanManageClassifications { get; }
    public bool CanManageFormalFrameworks { get; }
    public bool CanManageKeys { get; }
    public bool CanManageRelations { get; }
    public bool CanManageLabels { get; }
    public bool CanManageRegulations { get; }
    public bool CanManageKbo { get; }
    public bool CanManageVlimpers { get; }

    /// <summary>
    /// Whether the caller may view (but not necessarily manage) this
    /// organisation's KBO-koppeling. True when the caller holds the
    /// standalone read-only <see cref="Permission.CanViewOrganisationKbo"/>
    /// (granted to AlgemeenBeheerder/Developer and RegelgevingBeheerder —
    /// see <c>ui-permission-matrix.md</c>).
    /// </summary>
    public bool CanViewKbo { get; }

    /// <summary>
    /// Whether the caller may view (but not necessarily manage) this
    /// organisation's Vlimpers-koppeling. True when the caller holds the
    /// standalone read-only <see cref="Permission.CanViewOrganisationVlimpers"/>
    /// (granted to AlgemeenBeheerder/Developer and RegelgevingBeheerder).
    /// </summary>
    public bool CanViewVlimpers { get; }

    private OrganisationPermissions(
        bool canEdit,
        bool canDelete,
        bool canManageChildren,
        bool canManageContacts,
        bool canManageLocations,
        bool canManageBuildings,
        bool canManageFunctions,
        bool canManageCapacities,
        bool canViewFunctions,
        bool canViewCapacities,
        bool canManageNames,
        bool canManageClassifications,
        bool canManageFormalFrameworks,
        bool canManageKeys,
        bool canManageRelations,
        bool canManageLabels,
        bool canManageRegulations,
        bool canManageKbo,
        bool canManageVlimpers,
        bool canViewKbo,
        bool canViewVlimpers)
    {
        CanEdit = canEdit;
        CanDelete = canDelete;
        CanManageChildren = canManageChildren;
        CanManageContacts = canManageContacts;
        CanManageLocations = canManageLocations;
        CanManageBuildings = canManageBuildings;
        CanManageFunctions = canManageFunctions;
        CanManageCapacities = canManageCapacities;
        CanViewFunctions = canViewFunctions;
        CanViewCapacities = canViewCapacities;
        CanManageNames = canManageNames;
        CanManageClassifications = canManageClassifications;
        CanManageFormalFrameworks = canManageFormalFrameworks;
        CanManageKeys = canManageKeys;
        CanManageRelations = canManageRelations;
        CanManageLabels = canManageLabels;
        CanManageRegulations = canManageRegulations;
        CanManageKbo = canManageKbo;
        CanManageVlimpers = canManageVlimpers;
        CanViewKbo = canViewKbo;
        CanViewVlimpers = canViewVlimpers;
    }

    public static OrganisationPermissions For(IUser user, string ovoNumber, bool isUnderVlimpersManagement)
    {
        var userContext = new UserContext(user);
        var organisationContext = new OrganisationContext(ovoNumber);

        bool Satisfies(Permission permission, params IRestrictionContext[] contexts)
            => user.IsSatisfiedFor(permission, contexts);

        var canEditOrganisation =
            new OrganisationPolicy(Permission.CanManageOrganisation, ovoNumber, isUnderVlimpersManagement)
                .Check(user)
                .IsSuccessful
            || new OrganisationPolicy(
                    Permission.CanManageOrganisationInfoLimitedToVlimpers, ovoNumber, isUnderVlimpersManagement)
                .Check(user)
                .IsSuccessful
            || new OrganisationPolicy(
                    Permission.CanManageOrganisationInfoNotLimitedToVlimpers, ovoNumber, isUnderVlimpersManagement)
                .Check(user)
                .IsSuccessful;

        return new OrganisationPermissions(
            canEdit: canEditOrganisation,
            canDelete: user.HasPermission(Permission.CanTerminateOrganisation),
            canManageChildren: Satisfies(
                Permission.CanManageChildren,
                userContext,
                organisationContext,
                new VlimpersManagementContext(isUnderVlimpersManagement)),
            canManageContacts: Satisfies(Permission.CanManageContacts, userContext, organisationContext),
            canManageLocations: Satisfies(Permission.CanManageLocations, userContext, organisationContext),
            canManageBuildings: Satisfies(Permission.CanManageBuildings, userContext, organisationContext),
            canManageFunctions: Satisfies(Permission.CanManageFunctions, userContext, organisationContext),
            canManageCapacities: Satisfies(
                Permission.CanManageCapacities,
                userContext,
                organisationContext,
                new CapacityContext(Array.Empty<Guid>())),
            canViewFunctions:
                Satisfies(Permission.CanManageFunctions, userContext, organisationContext)
                || Satisfies(Permission.CanViewOrganisationFunctions, userContext, organisationContext),
            canViewCapacities:
                Satisfies(
                    Permission.CanManageCapacities,
                    userContext,
                    organisationContext,
                    new CapacityContext(Array.Empty<Guid>()))
                || Satisfies(Permission.CanViewOrganisationCapacities, userContext, organisationContext),
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
            canManageVlimpers: Satisfies(Permission.CanManageVlimpers),
            canViewKbo: Satisfies(Permission.CanViewOrganisationKbo),
            canViewVlimpers: Satisfies(Permission.CanViewOrganisationVlimpers));
    }
}
