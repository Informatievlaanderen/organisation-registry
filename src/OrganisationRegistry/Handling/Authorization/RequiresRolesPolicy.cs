namespace OrganisationRegistry.Handling.Authorization
{
    using System;
    using Infrastructure.Authorization;
    using Infrastructure.Domain;
    using Organisation.Exceptions;

    class RequiresRolesPolicy : ISecurityPolicy
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
        public AuthenticationResult Check(IUser user, ISession session)
        {
            foreach (var role in _roles)
            {
                if (user.IsInRole(role))
                    return AuthenticationResult.Success();
            }
            return AuthenticationResult.Fail(new InsufficientRights());
        }
    }
}
