namespace OrganisationRegistry.Organisation.Commands
{
    using System;
    using System.Security.Claims;

    public class TerminateOrganisation : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;
        public DateTime DateOfTermination { get; }
        public bool ForceTermination { get; }
        public ClaimsPrincipal User { get; }

        public TerminateOrganisation(
            OrganisationId organisationId,
            DateTime dateOfTermination,
            bool forceTermination,
            ClaimsPrincipal user)
        {
            Id = organisationId;
            DateOfTermination = dateOfTermination;
            ForceTermination = forceTermination;
            User = user;
        }
    }
}
