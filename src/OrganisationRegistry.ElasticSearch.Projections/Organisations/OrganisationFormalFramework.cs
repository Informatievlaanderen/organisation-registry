namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Client;
    using ElasticSearch.Organisations;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using FormalFramework.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Common;

    public class OrganisationFormalFramework :
        BaseProjection<OrganisationFormalFramework>,
        IEventHandler<OrganisationFormalFrameworkAdded>,
        IEventHandler<OrganisationFormalFrameworkUpdated>,
        IEventHandler<FormalFrameworkUpdated>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationCouplingWithKboCancelled>
    {
        private readonly Elastic _elastic;

        public OrganisationFormalFramework(
            ILogger<OrganisationFormalFramework> logger,
            Elastic elastic) : base(logger)
        {
            _elastic = elastic;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.FormalFrameworks.Single().FormalFrameworkId, message.Body.FormalFrameworkId,
                    "formalFrameworks", "formalFrameworkId",
                    "formalFrameworkName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            MassUpdateOrganisationFormalFrameworkParentName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            MassUpdateOrganisationFormalFrameworkParentName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            MassUpdateOrganisationFormalFrameworkParentName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling, message.Number, message.Timestamp);
        }

        private void MassUpdateOrganisationFormalFrameworkParentName(Guid bodyOrganisationId, string bodyName, int messageNumber, DateTimeOffset dateTimeOffset)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.FormalFrameworks.Single().ParentOrganisationId, bodyOrganisationId,
                    "formalFrameworks", "parentOrganisationId",
                    "parentOrganisationName", bodyName,
                    messageNumber,
                    dateTimeOffset));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkAdded> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            if (organisationDocument.FormalFrameworks == null)
                organisationDocument.FormalFrameworks = new List<OrganisationDocument.OrganisationFormalFramework>();

            organisationDocument.FormalFrameworks.RemoveExistingListItems(x => x.OrganisationFormalFrameworkId == message.Body.OrganisationFormalFrameworkId);

            organisationDocument.FormalFrameworks.Add(
                new OrganisationDocument.OrganisationFormalFramework(
                    message.Body.OrganisationFormalFrameworkId,
                    message.Body.FormalFrameworkId,
                    message.Body.FormalFrameworkName,
                    message.Body.ParentOrganisationId,
                    message.Body.ParentOrganisationName,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkUpdated> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            organisationDocument.FormalFrameworks.RemoveExistingListItems(x => x.OrganisationFormalFrameworkId == message.Body.OrganisationFormalFrameworkId);

            organisationDocument.FormalFrameworks.Add(
                new OrganisationDocument.OrganisationFormalFramework(
                    message.Body.OrganisationFormalFrameworkId,
                    message.Body.FormalFrameworkId,
                    message.Body.FormalFrameworkName,
                    message.Body.ParentOrganisationId,
                    message.Body.ParentOrganisationName,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }
    }
}
