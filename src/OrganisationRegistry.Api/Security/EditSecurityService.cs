namespace OrganisationRegistry.Api.Security
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Bus;
    using OrganisationRegistry.Infrastructure.Configuration;

    public class EditSecurityService : IEditSecurityService
    {
        private readonly EditApiConfiguration _configuration;

        public EditSecurityService(IOptions<EditApiConfiguration> configuration)
        {
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
