namespace OrganisationRegistry.Handling.Authorization
{
    using System;
    using Infrastructure.Authorization;
    using Organisation.Exceptions;

    public class RequiresRolesPolicy : ISecurityPolicy
    {
        private readonly Role[] _roles;

        public RequiresRolesPolicy(params Role[] roles)
        {
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));

            if (roles.Length == 0)
                throw new ArgumentException("At least one role needs to be provided", nameof(roles));

            _roles = roles;
        }
        public AuthorizationResult Check(IUser user)
        {
            foreach (var role in _roles)
            {
                if (user.IsInRole(role))
                    return AuthorizationResult.Success();
            }
            return AuthorizationResult.Fail(new InsufficientRights());
        }
    }
}
