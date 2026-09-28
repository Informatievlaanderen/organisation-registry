namespace OrganisationRegistry.Infrastructure.Authorization;

using System;
using System.Security.Claims;
using System.Threading.Tasks;

public interface ISecurityService
{
    Task<SecurityInformation> GetSecurityInformation(ClaimsPrincipal? user);

    Task<IUser> GetRequiredUser(ClaimsPrincipal? principal);
    Task<IUser> GetUser(ClaimsPrincipal? principal);
    void ExpireUserCache(string acmId);
}
