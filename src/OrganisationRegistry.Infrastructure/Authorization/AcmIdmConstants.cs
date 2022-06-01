namespace OrganisationRegistry.Infrastructure.Authorization;

using System;

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
    }

    public static class Roles
    {
        public const string AlgemeenBeheerder = "algemeenbeheerder";
        public const string VlimpersBeheerder = "vlimpersbeheerder";
        public const string RegelgevingBeheerder = "regelgevingbeheerder";

        [Obsolete("will be overruled with decentraalbeheerder")]
        public const string Beheerder = "beheerder";

        public const string DecentraalBeheerder = "decentraalbeheerder";
        public const string OrgaanBeheerder = "orgaanbeheerder";
    }

    public const string RolePrefix = "wegwijsbeheerder-";
}
