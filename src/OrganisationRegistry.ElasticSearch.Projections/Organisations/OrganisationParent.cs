namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ElasticSearch.Organisations;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Common;
    using Infrastructure.Change;

    public class OrganisationParent :
        BaseProjection<OrganisationParent>,
        IElasticEventHandler<OrganisationParentAdded>,
        IElasticEventHandler<OrganisationParentUpdated>,
        IElasticEventHandler<OrganisationInfoUpdated>,
        IElasticEventHandler<OrganisationInfoUpdatedFromKbo>,
        IElasticEventHandler<OrganisationCouplingWithKboCancelled>,
        IElasticEventHandler<OrganisationTerminated>
    {
        public OrganisationParent(
            ILogger<OrganisationParent> logger) : base(logger)
        {
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            return await MassUpdateOrganisationParentName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            return await MassUpdateOrganisationParentName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            return await MassUpdateOrganisationParentName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling, message.Number, message.Timestamp);
        }

        private static async Task<IElasticChange> MassUpdateOrganisationParentName(Guid organisationId, string name, int number, DateTimeOffset timestamp)
        {
            return new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic
                    .MassUpdateOrganisationAsync(
                        x => x.Parents.Single().ParentOrganisationId, organisationId,
                        "parents", "parentOrganisationId",
                        "parentOrganisationName", name,
                        number,
                        timestamp))
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentAdded> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.Parents == null)
                        document.Parents = new List<OrganisationDocument.OrganisationParent>();

                    document.Parents.RemoveExistingListItems(x => x.OrganisationOrganisationParentId == message.Body.OrganisationOrganisationParentId);

                    document.Parents.Add(
                        new OrganisationDocument.OrganisationParent(
                            message.Body.OrganisationOrganisationParentId,
                            message.Body.ParentOrganisationId,
                            message.Body.ParentOrganisationName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentUpdated> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Parents.RemoveExistingListItems(x => x.OrganisationOrganisationParentId == message.Body.OrganisationOrganisationParentId);

                    document.Parents.Add(
                        new OrganisationDocument.OrganisationParent(
                            message.Body.OrganisationOrganisationParentId,
                            message.Body.ParentOrganisationId,
                            message.Body.ParentOrganisationName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    foreach (var (key, value) in message.Body.FieldsToTerminate.Parents)
                    {
                        var organisationParent =
                            document
                                .Parents
                                .Single(x => x.OrganisationOrganisationParentId == key);

                        organisationParent.Validity.End = value;
                    }
                }
            );
        }
    }
}
