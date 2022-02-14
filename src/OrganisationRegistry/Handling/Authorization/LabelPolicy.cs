namespace OrganisationRegistry.Handling.Authorization
{
    using System;
    using System.Linq;
    using Configuration;
    using Infrastructure.Authorization;
    using Infrastructure.Domain;
    using Organisation;
    using Organisation.Exceptions;

    public class LabelPolicy : ISecurityPolicy
    {
        private readonly string _ovoNumber;
        private readonly bool _underVlimpersManagement;
        private readonly Guid _labelTypeId;
        private readonly IOrganisationRegistryConfiguration _configuration;

        public LabelPolicy(
            string ovoNumber,
            bool underVlimpersManagement,
            Guid labelTypeId,
            IOrganisationRegistryConfiguration configuration)
        {
            _ovoNumber = ovoNumber;
            _underVlimpersManagement = underVlimpersManagement;
            _labelTypeId = labelTypeId;
            _configuration = configuration;
        }

        public AuthorizationResult Check(IUser user)
        {
            if(user.IsInRole(Role.OrganisationRegistryBeheerder))
                return AuthorizationResult.Success();

            var labelIdsAllowedByVlimpers = _configuration.Authorization.LabelIdsAllowedForVlimpers;

            if (_underVlimpersManagement &&
                user.IsInRole(Role.VlimpersBeheerder) &&
                labelIdsAllowedByVlimpers.Contains(_labelTypeId))
                return AuthorizationResult.Success();

            if(!_underVlimpersManagement &&
               user.IsInRole(Role.OrganisatieBeheerder) &&
               user.Organisations.Contains(_ovoNumber))
                return AuthorizationResult.Success();

            return AuthorizationResult.Fail(new InsufficientRights());
        }
    }
}
