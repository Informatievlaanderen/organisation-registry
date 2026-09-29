namespace OrganisationRegistry.Api.Backoffice.Organisation;

/// <summary>
/// Nested permissions block returned on organisation sub-resource list items
/// that only support editing, not deleting (Classificaties, Benamingen,
/// Sleutels, ...). Exposes the caller's edit rights for that specific row
/// so the UI can gate actions without re-deriving authorization.
///
/// For sub-resources that also expose a DELETE endpoint (Hoedanigheden,
/// Toepassingsgebieden), use <see cref="ResourceEditAndDeletePermissions"/>
/// instead.
/// </summary>
public class ResourceEditPermissions
{
    public bool CanEdit { get; }

    public ResourceEditPermissions(bool canEdit)
    {
        CanEdit = canEdit;
    }
}
