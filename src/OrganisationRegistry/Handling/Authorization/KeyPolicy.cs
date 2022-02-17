namespace OrganisationRegistry.Handling.Authorization
{
    using System;
    using System.Linq;
    using Configuration;
    using Infrastructure.Authorization;
    using Infrastructure.Domain;
    using Organisation;
    using Organisation.Exceptions;

    public class KeyPolicy : ISecurityPolicy
    {
        private readonly string _ovoNumber;
        private readonly bool _underVlimpersManagement;
        private readonly Guid _keyTypeId;
        private readonly IOrganisationRegistryConfiguration _configuration;

        public KeyPolicy(
            string ovoNumber,
            bool underVlimpersManagement,
            Guid keyTypeId,
            IOrganisationRegistryConfiguration configuration)
        {
            _ovoNumber = ovoNumber;
            _underVlimpersManagement = underVlimpersManagement;
            _keyTypeId = keyTypeId;
            _configuration = configuration;
        }

        public AuthorizationResult Check(IUser user)
        {
            if(user.IsInRole(Role.OrganisationRegistryBeheerder))
                return AuthorizationResult.Success();

            var keyIdsAllowedForVlimpers = _configuration.Authorization.KeyIdsAllowedForVlimpers;
            var keyIdsAllowedOnlyForOrafin = _configuration.Authorization.KeyIdsAllowedOnlyForOrafin;

            if(keyIdsAllowedOnlyForOrafin.Contains(_keyTypeId)
               && !user.IsInRole(Role.Orafin))
                return AuthorizationResult.Fail(new InsufficientRights());

            if (_underVlimpersManagement &&
                user.IsInRole(Role.VlimpersBeheerder) &&
                keyIdsAllowedForVlimpers.Contains(_keyTypeId))
                return AuthorizationResult.Success();

            if(!_underVlimpersManagement &&
               user.IsInRole(Role.OrganisatieBeheerder) &&
               user.Organisations.Contains(_ovoNumber))
                return AuthorizationResult.Success();

            return AuthorizationResult.Fail(new InsufficientRights());
        }
    }
}
