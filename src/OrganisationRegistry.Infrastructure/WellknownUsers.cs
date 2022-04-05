namespace OrganisationRegistry.Infrastructure
{
    using System;
    using Authorization;

    public class WellknownUsers
    {
        public static User ScheduledCommandsService => Create("ScheduledCommandsService", Role.AutomatedTask);
        public static User KboSyncService => Create("KboSyncService", Role.AutomatedTask);

        public static User Orafin => Create("Orafin", "Edit Api", "Orafin Edit Api", Role.Orafin);

        public static User Magda => Create("Magda", "Reregistrator", "Magda Reregistrator", Role.AutomatedTask);

        public static User Nobody => Create();

        private static User Create(string name = "", Role? role = null) => Create(name, name, name, role);

        private static User Create(string firstName, string lastName, string userId, Role? role = null) =>
            new(firstName, lastName, userId, string.Empty, GetRoles(role), Array.Empty<string>());

        private static Role[] GetRoles(Role? role) =>
            role != null ? new[] { role.Value } : Array.Empty<Role>();
    }
}
