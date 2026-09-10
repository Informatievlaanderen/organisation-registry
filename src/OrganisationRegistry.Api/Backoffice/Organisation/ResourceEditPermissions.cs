namespace OrganisationRegistry.Api.Backoffice.Organisation;

/// <summary>
/// Nested permissions block returned on organisation sub-resource list items
/// (keys, labels, capacities, ...). Exposes the caller's edit rights for that
/// specific row so the UI can gate actions without re-deriving authorization.
/// </summary>
public class ResourceEditPermissions
{
    public bool CanEdit { get; }

    public ResourceEditPermissions(bool canEdit)
    {
        CanEdit = canEdit;
    }
}
