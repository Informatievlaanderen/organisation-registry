namespace OrganisationRegistry.Api.Kbo
{
    using System;
    using System.Linq;
    using Autofac.Features.OwnedInstances;
    using Configuration;
    using Microsoft.Extensions.Options;
    using SqlServer.Infrastructure;
    using SqlServer.OrganisationClassification;
    using OrganisationRegistry.Organisation;
    using SqlServer;

    public class KboOrganisationClassificationRetriever : IKboOrganisationClassificationRetriever
    {
        private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;
        private readonly IContextFactory _contextFactory;
        public KboOrganisationClassificationRetriever(
            IOrganisationRegistryConfiguration organisationRegistryConfiguration,
            IContextFactory contextFactory)
        {
            _organisationRegistryConfiguration = organisationRegistryConfiguration;
            _contextFactory = contextFactory;
        }

        public Guid? FetchOrganisationClassificationForLegalFormCode(string legalFormCode)
        {
            using (var context = _contextFactory.Create())
                return context.OrganisationClassificationList
                    .FirstOrDefault(o =>
                        o.OrganisationClassificationTypeId == _organisationRegistryConfiguration.KboV2LegalFormOrganisationClassificationTypeId &&
                        o.ExternalKey == legalFormCode)
                    ?.Id;
        }
    }
}
