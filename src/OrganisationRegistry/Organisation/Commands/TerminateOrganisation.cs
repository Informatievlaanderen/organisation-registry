namespace OrganisationRegistry.Organisation.Commands
{
    using System;
    using System.Security.Claims;

    public class TerminateOrganisation : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;
        public DateTime DateOfTermination { get; }

        public ClaimsPrincipal User { get; }


        public TerminateOrganisation(
            OrganisationId organisationId,
            DateTime dateOfTermination,
            ClaimsPrincipal user)
        {
            Id = organisationId;
            DateOfTermination = dateOfTermination;
            User = user;
        }
    }
}
