namespace OrganisationRegistry.Infrastructure.Authorization
{
    using System;
    using System.Security.Claims;

    /// <summary>
    /// Security service for use in event handlers only.
    /// Direct consequence of having commandbus and eventpublisher in the same class,
    /// and adding the ISecurityService dependency in there (which is only used during command handling).
    /// Possible fix could be to separate event handling/replace with another library (eg: Projac).
    /// </summary>
    public class NotImplementedSecurityService: ISecurityService
    {
        public bool CanAddOrganisation(ClaimsPrincipal user, Guid? parentOrganisationId)
        {
            throw new NotImplementedException();
        }

        public bool CanEditOrganisation(ClaimsPrincipal user, Guid organisationId)
        {
            throw new NotImplementedException();
        }

        public bool CanAddBody(ClaimsPrincipal user, Guid? organisationId)
        {
            throw new NotImplementedException();
        }

        public bool CanEditBody(ClaimsPrincipal user, Guid bodyId)
        {
            throw new NotImplementedException();
        }

        public bool CanEditDelegation(ClaimsPrincipal user, Guid? organisationId, Guid? bodyId)
        {
            throw new NotImplementedException();
        }

        public SecurityInformation GetSecurityInformation(ClaimsPrincipal user)
        {
            throw new NotImplementedException();
        }

        public Role[] GetRoles(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public IUser GetUser(ClaimsPrincipal? principal)
        {
            throw new NotImplementedException();
        }

        public bool CanUseKeyType(ClaimsPrincipal user, Guid keyTypeId)
        {
            throw new NotImplementedException();
        }

        public bool CanUseKeyType(IUser user, Guid keyTypeId)
        {
            throw new NotImplementedException();
        }
    }
}
