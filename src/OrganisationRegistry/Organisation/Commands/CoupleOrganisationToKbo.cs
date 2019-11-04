namespace OrganisationRegistry.Organisation.Commands
{
    using System.Security.Claims;

    public class CoupleOrganisationToKbo : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public KboNumber KboNumber { get; }
        public ClaimsPrincipal User { get; }

        public CoupleOrganisationToKbo(
            OrganisationId organisationId,
            KboNumber kboNumber,
            ClaimsPrincipal user)
        {
            Id = organisationId;

            KboNumber = kboNumber;
            User = user;
        }
    }
}
