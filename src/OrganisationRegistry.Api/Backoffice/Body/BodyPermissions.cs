namespace OrganisationRegistry.Api.Backoffice.Body;

using System;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;

/// <summary>
/// Per-body (orgaan) capability summary for the current caller, returned on body
/// list items so the UI can gate tabs and actions without re-deriving
/// authorization.
///
/// Each feature flag answers "may this user manage this feature on this body?".
/// A holder of an unrestricted grant (AlgemeenBeheerder, OrgaanBeheerder, ...)
/// passes for every body; a DecentraalBeheerder only passes for a body that
/// belongs to their own organisation or a child organisation (i.e. present in
/// <see cref="IUser.Bodies"/>).
/// </summary>
public class BodyPermissions
{
    public bool CanEdit { get; }
    public bool CanDelete { get; }
    public bool CanManageContacts { get; }
    public bool CanManageSeats { get; }
    public bool CanManageMandates { get; }
    public bool CanManageLifecycle { get; }
    public bool CanManageOrganisations { get; }
    public bool CanManageFormalFrameworks { get; }
    public bool CanManageMep { get; }
    public bool CanManageClassifications { get; }

    private BodyPermissions(
        bool canEdit,
        bool canDelete,
        bool canManageContacts,
        bool canManageSeats,
        bool canManageMandates,
        bool canManageLifecycle,
        bool canManageOrganisations,
        bool canManageFormalFrameworks,
        bool canManageMep,
        bool canManageClassifications)
    {
        CanEdit = canEdit;
        CanDelete = canDelete;
        CanManageContacts = canManageContacts;
        CanManageSeats = canManageSeats;
        CanManageMandates = canManageMandates;
        CanManageLifecycle = canManageLifecycle;
        CanManageOrganisations = canManageOrganisations;
        CanManageFormalFrameworks = canManageFormalFrameworks;
        CanManageMep = canManageMep;
        CanManageClassifications = canManageClassifications;
    }

    public static BodyPermissions For(IUser user, Guid bodyId)
    {
        var userContext = new UserContext(user);
        var bodyContext = new BodyContext(bodyId);

        bool Satisfies(Permission permission)
            => user.IsSatisfiedFor(permission, userContext, bodyContext);

        return new BodyPermissions(
            canEdit: Satisfies(Permission.CanManageBodies),
            canDelete: false,
            canManageContacts: Satisfies(Permission.BodiesCanManageContacts),
            canManageSeats: Satisfies(Permission.BodiesCanManageSeats),
            canManageMandates: Satisfies(Permission.BodiesCanManageMandates),
            canManageLifecycle: Satisfies(Permission.BodiesCanManageLifecycles),
            canManageOrganisations: Satisfies(Permission.BodiesCanManageOrganisations),
            canManageFormalFrameworks: Satisfies(Permission.BodiesCanManageFormalFrameworks),
            canManageMep: Satisfies(Permission.BodiesCanManageMep),
            canManageClassifications: Satisfies(Permission.BodiesCanManageClassifications));
    }
}
