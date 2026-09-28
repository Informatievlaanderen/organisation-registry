namespace OrganisationRegistry.Infrastructure;

using System;
using Authorization;

public class WellknownUsers
{
    public static User ScheduledCommandsService => Create("ScheduledCommandsService", Permission.CanRunScheduledJobs);
    public static User SyncRemovedItemsService => Create("SyncRemovedItemsService", Permission.CanRunScheduledJobs);
    public static User KboSyncService => Create("KboSyncService", Permission.CanManageKbo);
    public static User Orafin => Create("Orafin", "Edit Api", "Orafin Edit Api", Permission.CanReadOrafin);
    public static User Cjm => Create("Cjm", "Edit Api", "Cjm Edit Api", Permission.CanManageRegulations, Permission.CanManageLabels, Permission.CanManageBodies, Permission.CanManageKbo, Permission.BodiesCanManageContacts, Permission.BodiesCanManageSeats, Permission.BodiesCanManageMandates, Permission.BodiesCanManageLifecycles, Permission.BodiesCanManageOrganisations, Permission.BodiesCanManageClassifications, Permission.BodiesCanManageFormalFrameworks, Permission.BodiesCanManageMep);
    public static User TestClient => Create("TestClient", Permission.CanManageOrganisation);

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
}
