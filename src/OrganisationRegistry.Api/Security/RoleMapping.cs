namespace OrganisationRegistry.Api.Security;

using System.Collections.Generic;
using OrganisationRegistry.Infrastructure.Authorization;

public static class RoleMapping
{
    private const string VlimpersBeheerder = "vlimpersBeheerder";
    private const string DecentraalBeheerder = "decentraalBeheerder";
    private const string OrgaanBeheerder = "orgaanBeheerder";
    private const string AutomatedTask = "automatedTask";
    private const string CjmBeheerder = "cjmBeheerder";
    private const string AlgemeenBeheerder = "algemeenBeheerder";
    private const string Developer = "developer";
    private const string RegelgevingBeheerder = "regelgevingBeheerder";

    private static readonly Dictionary<Role, string> Mapping = new()
    {
        { Role.AlgemeenBeheerder, AlgemeenBeheerder },
        { Role.VlimpersBeheerder, VlimpersBeheerder },
        { Role.DecentraalBeheerder, DecentraalBeheerder },
        { Role.RegelgevingBeheerder, RegelgevingBeheerder },
        { Role.OrgaanBeheerder, OrgaanBeheerder },
        { Role.Developer, Developer },
        { Role.AutomatedTask, AutomatedTask },
        { Role.CjmBeheerder, CjmBeheerder },
    };

    private static readonly Dictionary<string, Role> ReverseMapping = new()
    {
        { AlgemeenBeheerder, Role.AlgemeenBeheerder },
        { VlimpersBeheerder, Role.VlimpersBeheerder },
        { DecentraalBeheerder, Role.DecentraalBeheerder },
        { RegelgevingBeheerder, Role.RegelgevingBeheerder },
        { OrgaanBeheerder, Role.OrgaanBeheerder },
        { Developer, Role.Developer },
        { AutomatedTask, Role.AutomatedTask },
        { CjmBeheerder, Role.CjmBeheerder },
    };

    public static string Map(Role role)
        => Mapping[role];

    public static Role Map(string role)
        => ReverseMapping[role];

    public static bool Exists(string role)
        => ReverseMapping.ContainsKey(role);
}
