namespace OrganisationRegistry.Infrastructure.Authorization
{
    using System;
    using System.Security.Claims;

    public interface ISecurityService
    {
        bool CanAddOrganisation(ClaimsPrincipal user, Guid? parentOrganisationId);
        bool CanEditOrganisation(ClaimsPrincipal user, Guid organisationId);

        bool CanAddBody(ClaimsPrincipal user, Guid? organisationId);
        bool CanEditBody(ClaimsPrincipal user, Guid bodyId);

        bool CanEditDelegation(ClaimsPrincipal user, Guid? organisationId, Guid? bodyId);

        SecurityInformation GetSecurityInformation(ClaimsPrincipal user);
        Role[] GetRoles(ClaimsPrincipal principal);

        IUser GetRequiredUser(ClaimsPrincipal? principal);
        IUser GetUser(ClaimsPrincipal? principal);
        bool CanUseKeyType(IUser user, Guid keyTypeId);
        bool CanUseLabelType(IUser user, Guid labelTypeId);
    }
}
