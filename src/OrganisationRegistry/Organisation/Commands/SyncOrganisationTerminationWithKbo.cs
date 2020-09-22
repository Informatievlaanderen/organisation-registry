namespace OrganisationRegistry.Organisation.Commands
{
    using System.Security.Claims;

    public class SyncOrganisationTerminationWithKbo : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public KboNumber KboNumber { get; }
        public ClaimsPrincipal User { get; }

        public SyncOrganisationTerminationWithKbo(
            OrganisationId organisationId,
            ClaimsPrincipal user)
        {
            Id = organisationId;

            User = user;
        }
    }
}
