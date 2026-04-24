namespace OrganisationRegistry.Api.Security;

using OrganisationRegistry.Infrastructure.Authorization;

public static class RoleMapping
{
    private static readonly BiDirectionalDictionary<Role, string> Mapping = new();

    static RoleMapping()
    {
        Mapping.Add(Role.AlgemeenBeheerder, AcmIdmConstants.Roles.AlgemeenBeheerder);
        Mapping.Add(Role.VlimpersBeheerder, AcmIdmConstants.Roles.VlimpersBeheerder);
        Mapping.Add(Role.DecentraalBeheerder, AcmIdmConstants.Roles.DecentraalBeheerder);
        Mapping.Add(Role.RegelgevingBeheerder, AcmIdmConstants.Roles.RegelgevingBeheerder);
        Mapping.Add(Role.OrgaanBeheerder, AcmIdmConstants.Roles.OrgaanBeheerder);
        Mapping.Add(Role.Developer, "developer");
        Mapping.Add(Role.AutomatedTask, "automatedTask");
        Mapping.Add(Role.CjmBeheerder, AcmIdmConstants.Roles.CjmBeheerder);
    }

    public static string Map(Role role)
        => Mapping[role];

    public static Role Map(string role)
        => Mapping[role];

    public static bool Exists(string role)
        => Mapping.Reverse.ContainsKey(role);
}
