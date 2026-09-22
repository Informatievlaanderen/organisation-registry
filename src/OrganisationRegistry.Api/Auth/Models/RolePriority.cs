namespace OrganisationRegistry.Api.Auth.Models;

using System.Collections.Generic;
using System.Linq;
using OrganisationRegistry.Infrastructure.Authorization;

/// <summary>
/// Prioriteitsvolgorde van applicatierollen, van meest naar minst belangrijk.
/// De <c>/v1/me</c> endpoint toont exact één rol: de belangrijkste die de
/// gebruiker bezit. De volgorde volgt bewust <em>niet</em> de declaratievolgorde
/// van <see cref="Role"/>. Developer staat als supergebruiker bovenaan.
/// CjmBeheerder en AutomatedTask zijn geen applicatierollen en worden hier nooit
/// als primaire rol geselecteerd.
/// </summary>
public static class RolePriority
{
    private static readonly Role[] Order =
    [
        Role.Developer,
        Role.AlgemeenBeheerder,
        Role.DecentraalBeheerder,
        Role.VlimpersBeheerder,
        Role.RegelgevingBeheerder,
        Role.OrgaanBeheerder,
        Role.Orafin,
    ];

    /// <summary>
    /// Selecteert de belangrijkste rol die de gebruiker bezit volgens
    /// <see cref="Order"/>. Geeft <c>null</c> terug wanneer de gebruiker geen
    /// enkele applicatierol bezit.
    /// </summary>
    public static Role? SelectPrimary(IEnumerable<Role> roles)
    {
        var owned = new HashSet<Role>(roles);
        foreach (var role in Order)
            if (owned.Contains(role))
                return role;

        return null;
    }
}
