namespace OrganisationRegistry.Infrastructure.Authorization
{
    using System;

    public interface IEditSecurityService
    {
        bool CanAddKey(Guid keyTypeId);
        bool CanEditKey(Guid keyTypeId);
    }
}
