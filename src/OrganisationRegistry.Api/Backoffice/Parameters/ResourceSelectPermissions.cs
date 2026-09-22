namespace OrganisationRegistry.Api.Backoffice.Parameters;

/// <summary>
/// Nested permissions block returned on reference-data list items (key types,
/// organisation classification types, ...). Exposes whether the caller may
/// select this value for the organisation given via <c>forOrganisationId</c>,
/// so the UI can gate dropdown options without re-deriving authorization.
/// </summary>
public class ResourceSelectPermissions
{
    public bool CanSelect { get; }

    public ResourceSelectPermissions(bool canSelect)
    {
        CanSelect = canSelect;
    }
}
