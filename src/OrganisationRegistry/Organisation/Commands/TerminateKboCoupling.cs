namespace OrganisationRegistry.Organisation.Commands
{
    using System.Security.Claims;

    public class TerminateKboCoupling : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public KboNumber KboNumber { get; }
        public ClaimsPrincipal User { get; }

        public TerminateKboCoupling(
            OrganisationId organisationId,
            ClaimsPrincipal user)
        {
            Id = organisationId;

            User = user;
        }
    }
}
