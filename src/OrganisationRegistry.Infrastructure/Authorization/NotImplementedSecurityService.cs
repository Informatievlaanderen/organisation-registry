namespace OrganisationRegistry.Infrastructure.Authorization;

using System;
using System.Security.Claims;
using System.Threading.Tasks;

/// <summary>
///     Security service for use in event handlers only.
///     Direct consequence of having commandbus and eventpublisher in the same class,
///     and adding the ISecurityService dependency in there (which is only used during command handling).
///     Possible fix could be to separate event handling/replace with another library (eg: Projac).
/// </summary>
public class NotImplementedSecurityService : ISecurityService
{
    public Task<SecurityInformation> GetSecurityInformation(ClaimsPrincipal? user)
        => throw new NotImplementedException();

    public Task<IUser> GetRequiredUser(ClaimsPrincipal? principal)
        => throw new NotImplementedException();

    public Task<IUser> GetUser(ClaimsPrincipal? principal)
        => throw new NotImplementedException();

    public void ExpireUserCache(string acmId)
    {
        throw new NotImplementedException();
    }

    public Role[] GetRoles(ClaimsPrincipal principal)
        => throw new NotImplementedException();
}
