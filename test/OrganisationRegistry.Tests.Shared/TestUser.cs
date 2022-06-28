namespace OrganisationRegistry.Tests.Shared;

using Infrastructure.Authorization;

public static class TestUser
{
    public static User AlgemeenBeheerder
        => new UserBuilder().AddRoles(Role.AlgemeenBeheerder).Build();

    public static User DecentraalBeheerder
        => new UserBuilder().AsDecentraalBeheerder().Build();

    public static User OrgaanBeheerder
        => new UserBuilder().AddRoles(Role.OrgaanBeheerder).Build();

    public static User RegelgevingBeheerder
        => new UserBuilder().AddRoles(Role.RegelgevingBeheerder).Build();

    public static User VlimpersBeheerder
        => new UserBuilder().AddRoles(Role.VlimpersBeheerder).Build();

    public static User User
        => new UserBuilder().Build();
}
