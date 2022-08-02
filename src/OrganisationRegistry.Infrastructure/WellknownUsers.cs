namespace OrganisationRegistry.Infrastructure;

using System;
using Authorization;

public class WellknownUsers
{
    public static User ScheduledCommandsService => Create("ScheduledCommandsService", Role.AutomatedTask);
    public static User SyncRemovedItemsService => Create("SyncRemovedItemsService", Role.AutomatedTask);
    public static User KboSyncService => Create("KboSyncService", Role.AutomatedTask);
    public static User Orafin => Create("Orafin", "Edit Api", "Orafin Edit Api", Role.Orafin);
    public static User Cjm => Create("Cjm", "Edit Api", "Cjm Edit Api", Role.CjmBeheerder);
    public static User TestClient => Create("TestClient", Role.AlgemeenBeheerder);

    public static User Magda => Create("Magda", "Reregistrator", "Magda Reregistrator", Role.AutomatedTask);

    public static User Nobody => Create();

    private static User Create(string name = "", Role? role = null) => Create(name, name, name, role);

    private static User Create(string firstName, string lastName, string userId, Role? role = null) =>
        new(firstName, lastName, userId, string.Empty, GetRoles(role), Array.Empty<string>(), Array.Empty<Guid>(), Array.Empty<Guid>());

    private static Role[] GetRoles(Role? role) =>
        role != null ? new[] { role.Value } : Array.Empty<Role>();
}
