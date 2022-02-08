namespace OrganisationRegistry.Handling.Authorization
{
    using System;
    using Infrastructure.Authorization;
    using Infrastructure.Domain;
    using Organisation;
    using Organisation.Exceptions;

    class VlimpersParentChildPolicy : ISecurityPolicy
    {
        private readonly OrganisationId _childOrganisationId;

        public VlimpersParentChildPolicy(OrganisationId childOrganisationId)
        {
            _childOrganisationId = childOrganisationId;
        }
        public AuthenticationResult Check(IUser user, ISession session)
        {
            if (user.IsInRole(Role.OrganisationRegistryBeheerder))
                return AuthenticationResult.Success();

            var child = session.Get<Organisation>(_childOrganisationId);

            if (child.State.UnderVlimpersManagement &&
                user.IsAuthorizedForVlimpersOrganisations)
                return AuthenticationResult.Success();

            if (!child.State.UnderVlimpersManagement &&
                user.Organisations.Contains(child.State.OvoNumber))
                return AuthenticationResult.Success();

            return child.State.UnderVlimpersManagement
                ? AuthenticationResult.Fail(new UserIsNotAuthorizedForVlimpersOrganisations())
                : AuthenticationResult.Fail(new InsufficientRights());
        }
    }
}
