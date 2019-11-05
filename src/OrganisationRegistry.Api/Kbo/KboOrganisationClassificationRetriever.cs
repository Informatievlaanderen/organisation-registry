namespace OrganisationRegistry.Api.Kbo
{
    using System;
    using System.Linq;
    using Configuration;
    using Microsoft.Extensions.Options;
    using SqlServer.Infrastructure;
    using SqlServer.OrganisationClassification;
    using OrganisationRegistry.Organisation;

    public class KboOrganisationClassificationRetriever : IKboOrganisationClassificationRetriever
    {
        private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;
        private readonly Func<OrganisationRegistryContext> _contextFactory;

        public KboOrganisationClassificationRetriever(
            IOrganisationRegistryConfiguration organisationRegistryConfiguration,
            Func<OrganisationRegistryContext> contextFactory
            )
        {
            _organisationRegistryConfiguration = organisationRegistryConfiguration;
            _contextFactory = contextFactory;
        }

        public Guid? FetchOrganisationClassificationForLegalFormCode(string legalFormCode)
        {
            using (var context = _contextFactory())
                return context.OrganisationClassificationList
                    .FirstOrDefault(o => MatchesLegalFormCode(legalFormCode, o))
                    ?.Id;
        }

        private bool MatchesLegalFormCode(string legalFormCode, OrganisationClassificationListItem o)
        {
            return o.OrganisationClassificationTypeId == _organisationRegistryConfiguration.KboV2LegalFormOrganisationClassificationTypeId &&
                   o.ExternalKey == legalFormCode;
        }
    }
}
