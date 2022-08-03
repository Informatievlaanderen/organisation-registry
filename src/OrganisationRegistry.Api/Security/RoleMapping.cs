namespace OrganisationRegistry.Api.Security;

using OrganisationRegistry.Infrastructure.Authorization;

public static class RoleMapping
{
    private static readonly BiDirectionalDictionary<Role, string> Mapping = new();

    static RoleMapping()
    {
        Mapping.Add(Role.AlgemeenBeheerder, "algemeenBeheerder");
        Mapping.Add(Role.VlimpersBeheerder, "vlimpersBeheerder");
        Mapping.Add(Role.DecentraalBeheerder, "decentraalBeheerder");
        Mapping.Add(Role.RegelgevingBeheerder, "regelgevingBeheerder");
        Mapping.Add(Role.OrgaanBeheerder, "orgaanBeheerder");
        Mapping.Add(Role.Developer, "developer");
        Mapping.Add(Role.AutomatedTask, "automatedTask");
        Mapping.Add(Role.CjmBeheerder, "cjmBeheerder");
    }

    public static string Map(Role role)
        => Mapping[role];

    public static Role Map(string role)
        => Mapping[role];

    public static bool Exists(string role)
        => Mapping.ContainsKey(role);
}
