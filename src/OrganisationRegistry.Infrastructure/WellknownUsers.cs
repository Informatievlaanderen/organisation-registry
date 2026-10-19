namespace OrganisationRegistry.Infrastructure;

using System;
using Authorization;

public class WellknownUsers
{
    public static User ScheduledCommandsService => Create("ScheduledCommandsService", Permission.CanRunScheduledJobs);

    // Removes organisation keys/capacities that were scheduled for removal
    // (see SyncRemovedItemsService). This system account doesn't act "as" any
    // particular role/organisation — it cleans up scheduled removals
    // irrespective of Vlimpers-management or keytype/capacity restrictions —
    // so it needs the unrestricted grants (like AlgemeenBeheerder has), not a
    // restricted one: RemoveOrganisationKeyCommandHandler/
    // RemoveOrganisationCapacityCommandHandler route through the same
    // restriction-aware KeyPolicy/CapacityPolicy as Add/Update.
    public static User SyncRemovedItemsService => Create(
        "SyncRemovedItemsService",
        Permission.CanRunScheduledJobs,
        Permission.CanManageKeys,
        Permission.CanManageCapacities);
    public static User KboSyncService => Create("KboSyncService", Permission.CanManageKbo);
    // Permissions for client-credential users (TestClient/Orafin) are
    // derived from the token's scopes via ScopePermissionMap (the single
    // source of truth for scope → permission translation), not hardcoded
    // here — callers pass the already-resolved PermissionSet in.
    public static User Orafin(PermissionSet permissions) => CreateWithPermissions("Orafin", "Edit Api", "Orafin Edit Api", permissions);
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
