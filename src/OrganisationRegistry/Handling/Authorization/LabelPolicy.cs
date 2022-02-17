namespace OrganisationRegistry.Handling.Authorization
{
    using System;
    using System.Linq;
    using Configuration;
    using Infrastructure.Authorization;
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

            var isVlimpersLabel = IsVlimpersLabel();

            if (_underVlimpersManagement &&
                user.IsInRole(Role.VlimpersBeheerder) && isVlimpersLabel)
                return AuthorizationResult.Success();

            if (user.IsOrganisatieBeheerderFor(_ovoNumber))
            {
                if (_underVlimpersManagement && isVlimpersLabel)
                {
                    return AuthorizationResult.Fail(new InsufficientRights());
                }
                return AuthorizationResult.Success();
            }

            return AuthorizationResult.Fail(new InsufficientRights());
        }

        private bool IsVlimpersLabel()
        {
            var labelIdsAllowedByVlimpers = _configuration.Authorization.LabelIdsAllowedForVlimpers;
            var isVlimpersLabel = labelIdsAllowedByVlimpers.Contains(_labelTypeId);
            return isVlimpersLabel;
        }
    }
}
