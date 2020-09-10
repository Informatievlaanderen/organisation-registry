namespace OrganisationRegistry.Organisation.Commands
{
    using System.Security.Claims;

    public class CancelCouplingWithKbo : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public ClaimsPrincipal User { get; }

        public CancelCouplingWithKbo(
            OrganisationId organisationId,
            ClaimsPrincipal user)
        {
            Id = organisationId;

            User = user;
        }
    }
}
