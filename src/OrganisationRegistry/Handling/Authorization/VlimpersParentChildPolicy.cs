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
        public AuthorizationResult Check(IUser user, ISession session)
        {
            if (user.IsInRole(Role.OrganisationRegistryBeheerder))
                return AuthorizationResult.Success();

            var child = session.Get<Organisation>(_childOrganisationId);

            if (child.State.UnderVlimpersManagement &&
                user.IsAuthorizedForVlimpersOrganisations)
                return AuthorizationResult.Success();

            if (!child.State.UnderVlimpersManagement &&
                user.Organisations.Contains(child.State.OvoNumber))
                return AuthorizationResult.Success();

            return child.State.UnderVlimpersManagement
                ? AuthorizationResult.Fail(new UserIsNotAuthorizedForVlimpersOrganisations())
                : AuthorizationResult.Fail(new InsufficientRights());
        }
    }
}
