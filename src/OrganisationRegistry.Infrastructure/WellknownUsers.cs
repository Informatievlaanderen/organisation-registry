namespace OrganisationRegistry.Infrastructure;

using System;
using Authorization;

public class WellknownUsers
{
    public static User ScheduledCommandsService => Create("ScheduledCommandsService", Permission.CanRunScheduledJobs);
    public static User SyncRemovedItemsService => Create("SyncRemovedItemsService", Permission.CanRunScheduledJobs);
    public static User KboSyncService => Create("KboSyncService", Permission.CanManageKbo);
    // Permissions for client-credential users (TestClient/Cjm/Orafin) are
    // derived from the token's scopes via ScopePermissionMap (the single
    // source of truth for scope → permission translation), not hardcoded
    // here — callers pass the already-resolved PermissionSet in.
    public static User Orafin(PermissionSet permissions) => CreateWithPermissions("Orafin", "Edit Api", "Orafin Edit Api", permissions);
    public static User Cjm(PermissionSet permissions) => CreateWithPermissions("Cjm", "Edit Api", "Cjm Edit Api", permissions);
    public static User TestClient(PermissionSet permissions) => CreateWithPermissions("TestClient", "TestClient", "TestClient", permissions);

    public static User Magda => Create("Magda", "Reregistrator", "Magda Reregistrator", Permission.CanManageKbo);

    public static User Nobody => Create();

    private static User Create(string name = "", params Permission[] permissions) => Create(name, name, name, permissions);

    private static User Create(string firstName, string lastName, string userId, params Permission[] permissions)
        => new(
            firstName,
            lastName,
            userId,
            string.Empty,
            Array.Empty<string>(),
            Array.Empty<Guid>(),
            Array.Empty<Guid>(),
            PermissionSet.Of(permissions));

    private static User CreateWithPermissions(string firstName, string lastName, string userId, PermissionSet permissions)
        => new(
            firstName,
            lastName,
            userId,
            string.Empty,
            Array.Empty<string>(),
            Array.Empty<Guid>(),
            Array.Empty<Guid>(),
            permissions);
}
