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
        private readonly Guid[] _labelTypeIds;
        private readonly IOrganisationRegistryConfiguration _configuration;

        public LabelPolicy(string ovoNumber,
            bool underVlimpersManagement,
            IOrganisationRegistryConfiguration configuration,
            params Guid[] labelTypeIds)
        {
            _ovoNumber = ovoNumber;
            _underVlimpersManagement = underVlimpersManagement;
            _labelTypeIds = labelTypeIds;
            _configuration = configuration;
        }

        public AuthorizationResult Check(IUser user)
        {
            if(user.IsInRole(Role.OrganisationRegistryBeheerder))
                return AuthorizationResult.Success();

            var isVlimpersLabel = ContainsVlimpersLabel(_labelTypeIds);

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

        private bool ContainsVlimpersLabel(Guid[] labelTypeIds)
        {
            var labelIdsAllowedByVlimpers = _configuration.Authorization.LabelIdsAllowedForVlimpers;
            return labelTypeIds.Any(labelTypeId =>
                labelIdsAllowedByVlimpers.Contains(labelTypeId));
        }
    }
}
