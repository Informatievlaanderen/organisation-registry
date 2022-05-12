namespace OrganisationRegistry.Organisation
{
    using System;

    public class TerminateOrganisation : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;
        public DateTime DateOfTermination { get; }
        public bool ForceKboTermination { get; }

        public TerminateOrganisation(
            OrganisationId organisationId,
            DateTime dateOfTermination,
            bool forceKboTermination)
        {
            Id = organisationId;
            DateOfTermination = dateOfTermination;
            ForceKboTermination = forceKboTermination;
        }
    }
}
