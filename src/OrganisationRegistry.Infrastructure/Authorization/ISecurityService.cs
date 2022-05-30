namespace OrganisationRegistry.Infrastructure.Authorization;

using System;
using System.Security.Claims;
using System.Threading.Tasks;

public interface ISecurityService
{
    Task<bool> CanAddOrganisation(ClaimsPrincipal user, Guid? parentOrganisationId);
    Task<bool> CanEditOrganisation(ClaimsPrincipal user, Guid organisationId);

    Task<bool> CanAddBody(ClaimsPrincipal user, Guid? organisationId);
    Task<bool> CanEditBody(ClaimsPrincipal user, Guid bodyId);

    Task<bool> CanEditDelegation(ClaimsPrincipal user, Guid? organisationId, Guid? bodyId);

    Task<SecurityInformation> GetSecurityInformation(ClaimsPrincipal? user);

    Task<IUser> GetRequiredUser(ClaimsPrincipal? principal);
    Task<IUser> GetUser(ClaimsPrincipal? principal);
    bool CanUseKeyType(IUser user, Guid keyTypeId);
    bool CanUseLabelType(IUser user, Guid labelTypeId);
    void ExpireUserCache(string acmId);
}