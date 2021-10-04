namespace OrganisationRegistry.Infrastructure.Authorization
{
    using System;
    using System.Security.Claims;

    public interface IEditSecurityService
    {
        bool CanAddKey(Guid keyTypeId);
        bool CanEditKey(Guid keyTypeId);
    }
}
