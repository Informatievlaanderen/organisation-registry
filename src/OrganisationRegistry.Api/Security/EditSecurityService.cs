namespace OrganisationRegistry.Api.Security;

using System;
using Microsoft.Extensions.Options;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;

public class EditSecurityService : IEditSecurityService
{
    private readonly EditApiConfigurationSection _configurationSection;

    public EditSecurityService(IOptions<EditApiConfigurationSection> configuration)
    {
        _configurationSection = configuration.Value;
    }

    public bool CanAddKey(Guid keyTypeId)
    {
        return _configurationSection.Orafin.Equals(keyTypeId);
    }

    public bool CanEditKey(Guid keyTypeId)
    {
        return _configurationSection.Orafin.Equals(keyTypeId);
    }
}