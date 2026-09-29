namespace OrganisationRegistry.Api.Backoffice.Organisation;

/// <summary>
/// Nested permissions block returned on organisation sub-resource list items
/// that support both editing and deleting (Hoedanigheden/Capacity,
/// Toepassingsgebieden/FormalFramework). Exposes the caller's edit and
/// delete rights for that specific row so the UI can gate actions without
/// re-deriving authorization.
///
/// <see cref="CanDelete"/> mirrors <see cref="CanEdit"/>: the same
/// permission (and, where applicable, the same resource-scoped
/// restriction) guards both the PUT and DELETE endpoint on the
/// corresponding command controller, so there is no separate
/// delete-specific check to compute.
///
/// For sub-resources without a DELETE endpoint, use
/// <see cref="ResourceEditPermissions"/> instead.
/// </summary>
public class ResourceEditAndDeletePermissions
{
    public bool CanEdit { get; }
    public bool CanDelete { get; }

    public ResourceEditAndDeletePermissions(bool canEdit)
    {
        CanEdit = canEdit;
        CanDelete = canEdit;
    }
}
