namespace OrganisationRegistry.Infrastructure.Authorization
{
    public class OrganisationRegistryClaims
    {
        public const string ClaimRoles = "iv_wegwijs_rol_3D";
        public const string ClaimAcmId = "vo_id";
        public const string ClaimFirstname = "urn:be:vlaanderen:acm:voornaam";
        public const string ClaimName = "urn:be:vlaanderen:acm:familienaam";

        public const string ClaimOrganisation = "urn:be:vlaanderen:wegwijs:organisation";
        public const string ClaimUserId = "urn:be:vlaanderen:dienstverlening:acmid";
        public const string ClaimIp = "urn:be:vlaanderen:wegwijs:ip";

        public const string OrganisationRegistryBeheerderPrefix = "wegwijsbeheerder-";

        public const string OrganisationRegistryBeheerderRole = "algemeenbeheerder";
        public const string OrganisationRegistryInvoerderRole = "beheerder";
        public const string OrganisationRegistryOrgaanBeheerderRole = "orgaanbeheerder";
    }
}
