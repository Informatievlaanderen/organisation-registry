namespace OrganisationRegistry.Api.Security
{
    public enum Role
    {
        OrganisationRegistryBeheerder,
        OrganisatieBeheerder,
        OrgaanBeheerder,
        Developer,
        AutomatedTask
    }

    public static class Roles
    {
        public const string OrganisationRegistryBeheerder = "wegwijsBeheerder";
        public const string OrganisatieBeheerder = "organisatieBeheerder";
        public const string OrgaanBeheerder = "orgaanBeheerder";
        public const string Developer = "developer";
        public const string AutomatedTask = "automatedTask";
    }
}
