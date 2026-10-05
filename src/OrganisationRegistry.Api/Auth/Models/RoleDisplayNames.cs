namespace OrganisationRegistry.Api.Auth.Models;

using System.Collections.Generic;
using OrganisationRegistry.Infrastructure.Authorization;

/// <summary>
/// Human-readable Dutch display names for the <see cref="Role"/> the <c>/v1/me</c>
/// endpoint exposes to the frontend, as opposed to the bare enum member name
/// (e.g. <c>AlgemeenBeheerder</c>). These are the exact labels used in the UI/UX
/// copy and in <c>ui-permission-matrix.md</c>.
///
/// Roles that are never selected as a user's primary role by <see cref="RolePriority"/>
/// (<c>CjmBeheerder</c>, <c>AutomatedTask</c>) have no entry here; <see cref="For"/>
/// falls back to the bare enum name for any role without a mapping.
/// </summary>
public static class RoleDisplayNames
{
    private static readonly IReadOnlyDictionary<Role, string> Names = new Dictionary<Role, string>
    {
        [Role.VoMedewerker] = "VO medewerker",
        [Role.AlgemeenBeheerder] = "Algemeen beheerder",
        [Role.DecentraalBeheerder] = "Decentraal beheerder",
        [Role.VlimpersBeheerder] = "Vlimpers beheerder",
        [Role.OrgaanBeheerder] = "Orgaan beheerder",
        [Role.RegelgevingBeheerder] = "Regelgevingbeheerder en deugdelijk bestuur beheerder",
    };

    /// <summary>
    /// Resolves the Dutch display name for <paramref name="role"/>, falling back
    /// to <c>role.ToString()</c> when no display name is mapped.
    /// </summary>
    public static string For(Role role)
        => Names.TryGetValue(role, out var name) ? name : role.ToString();
}
