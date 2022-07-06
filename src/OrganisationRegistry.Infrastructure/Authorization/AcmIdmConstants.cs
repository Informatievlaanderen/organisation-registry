namespace OrganisationRegistry.Infrastructure.Authorization;

public static class AcmIdmConstants
{
    public static class Claims
    {
        public const string Role = "iv_wegwijs_rol_3D";
        public const string AcmId = "vo_id";
        public const string Firstname = "urn:be:vlaanderen:acm:voornaam";
        public const string FamilyName = "urn:be:vlaanderen:acm:familienaam";
        public const string Organisation = "urn:be:vlaanderen:wegwijs:organisation";
        public const string Id = "urn:be:vlaanderen:dienstverlening:acmid";
        public const string Ip = "urn:be:vlaanderen:wegwijs:ip";

        // OrgCode is unused for now
        public const string OrgCode = "dv_organisatieregister_orgcode";
        public const string Scope = "scope";

    }

    public static class Roles
    {
        public const string AlgemeenBeheerder = "algemeenbeheerder";
        public const string VlimpersBeheerder = "vlimpersbeheerder";
        public const string RegelgevingBeheerder = "regelgevingbeheerder";

        public const string DecentraalBeheerder = "decentraalbeheerder";
        public const string OrgaanBeheerder = "orgaanbeheerder";
    }

    public static class Scopes
    {
        public const string CjmBeheerder = "dv_organisatieregister_cjmbeheerder";
        public const string OrafinBeheerder = "dv_organisatieregister_orafinbeheerder";
        public const string Info  = "dv_organisatieregister_info";
        public const string TestClient  = "dv_organisatieregister_testclient";
    }

    public const string RolePrefix = "wegwijsbeheerder-";
}
