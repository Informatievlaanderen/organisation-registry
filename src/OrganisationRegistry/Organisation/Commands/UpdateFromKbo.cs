namespace OrganisationRegistry.Organisation.Commands
{
    using System;
    using System.Security.Claims;

    public class UpdateFromKbo : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public ClaimsPrincipal User { get; }
        public DateTime ModificationTime { get; }

        public UpdateFromKbo(OrganisationId organisationId,
            ClaimsPrincipal user,
            DateTimeOffset modificationTime)
        {
            User = user;
            ModificationTime = modificationTime.Date;
            Id = organisationId;
        }
    }
}
