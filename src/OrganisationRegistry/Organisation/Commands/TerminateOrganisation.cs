namespace OrganisationRegistry.Organisation.Commands
{
    using System;
    using System.Security.Claims;

    public class TerminateOrganisation : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;
        public DateTime DateOfTermination { get; }
        public bool ForceKboTermination { get; }
        public ClaimsPrincipal User { get; }

        public TerminateOrganisation(
            OrganisationId organisationId,
            DateTime dateOfTermination,
            bool forceKboTermination,
            ClaimsPrincipal user)
        {
            Id = organisationId;
            DateOfTermination = dateOfTermination;
            ForceKboTermination = forceKboTermination;
            User = user;
        }
    }
}
