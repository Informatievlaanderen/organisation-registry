namespace OrganisationRegistry.Api.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Bus;
    using OrganisationRegistry.Infrastructure.Configuration;
    using SqlServer.Infrastructure;

    public class EditSecurityService : IEditSecurityService
    {
        private readonly Dictionary<string, Role> _roleMapping = new Dictionary<string, Role>
        {
            { Roles.OrganisationRegistryBeheerder, Role.OrganisationRegistryBeheerder },
            { Roles.OrganisatieBeheerder, Role.OrganisatieBeheerder },
            { Roles.OrgaanBeheerder, Role.OrgaanBeheerder },
            { Roles.Developer, Role.Developer },
            { Roles.AutomatedTask, Role.AutomatedTask}
        };

        private const string ClaimOrganisation = "urn:be:vlaanderen:wegwijs:organisation";

        private readonly OrganisationRegistryContext _context;
        private readonly EditApiConfiguration _configuration;

        public EditSecurityService(
            OrganisationRegistryContext context,
            IOptions<EditApiConfiguration> configuration)
        {
            _context = context;
            _configuration = configuration.Value;
        }

        public bool CanAddKey(Guid keyTypeId)
        {
            return _configuration.Orafin.Equals(keyTypeId);
        }

        public bool CanEditKey(Guid keyTypeId)
        {
            return _configuration.Orafin.Equals(keyTypeId);
        }
    }
}
