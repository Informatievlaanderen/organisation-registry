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
    using FormalFramework.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Common;
    using Infrastructure.Change;

    public class OrganisationFormalFramework :
        BaseProjection<OrganisationFormalFramework>,
        IElasticEventHandler<OrganisationFormalFrameworkAdded>,
        IElasticEventHandler<OrganisationFormalFrameworkUpdated>,
        IElasticEventHandler<FormalFrameworkUpdated>,
        IElasticEventHandler<OrganisationInfoUpdated>,
        IElasticEventHandler<OrganisationNameUpdated>,
        IElasticEventHandler<OrganisationInfoUpdatedFromKbo>,
        IElasticEventHandler<OrganisationCouplingWithKboCancelled>,
        IElasticEventHandler<OrganisationTerminated>,
        IElasticEventHandler<OrganisationTerminatedV2>
    {
        public OrganisationFormalFramework(
            ILogger<OrganisationFormalFramework> logger) : base(logger)
        {
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic
                    .MassUpdateOrganisationAsync(
                        x => x.FormalFrameworks.Single().FormalFrameworkId, message.Body.FormalFrameworkId,
                        "formalFrameworks", "formalFrameworkId",
                        "formalFrameworkName", message.Body.Name,
                        message.Number,
                        message.Timestamp))
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            return await MassUpdateOrganisationFormalFrameworkParentName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationNameUpdated> message)
        {
            return await MassUpdateOrganisationFormalFrameworkParentName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            return await MassUpdateOrganisationFormalFrameworkParentName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            return await MassUpdateOrganisationFormalFrameworkParentName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling, message.Number, message.Timestamp);
        }

        private async Task<IElasticChange> MassUpdateOrganisationFormalFrameworkParentName(Guid bodyOrganisationId, string bodyName, int messageNumber, DateTimeOffset dateTimeOffset)
        {
            return new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic
                    .MassUpdateOrganisationAsync(
                        x => x.FormalFrameworks.Single().ParentOrganisationId, bodyOrganisationId,
                        "formalFrameworks", "parentOrganisationId",
                        "parentOrganisationName", bodyName,
                        messageNumber,
                        dateTimeOffset))
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkAdded> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.FormalFrameworks == null)
                        document.FormalFrameworks = new List<OrganisationDocument.OrganisationFormalFramework>();

                    document.FormalFrameworks.RemoveExistingListItems(x => x.OrganisationFormalFrameworkId == message.Body.OrganisationFormalFrameworkId);

                    document.FormalFrameworks.Add(
                        new OrganisationDocument.OrganisationFormalFramework(
                            message.Body.OrganisationFormalFrameworkId,
                            message.Body.FormalFrameworkId,
                            message.Body.FormalFrameworkName,
                            message.Body.ParentOrganisationId,
                            message.Body.ParentOrganisationName,
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkUpdated> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.FormalFrameworks.RemoveExistingListItems(x =>
                        x.OrganisationFormalFrameworkId == message.Body.OrganisationFormalFrameworkId);

                    document.FormalFrameworks.Add(
                        new OrganisationDocument.OrganisationFormalFramework(
                            message.Body.OrganisationFormalFrameworkId,
                            message.Body.FormalFrameworkId,
                            message.Body.FormalFrameworkName,
                            message.Body.ParentOrganisationId,
                            message.Body.ParentOrganisationName,
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
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

                    foreach (var (key, value) in message.Body.FieldsToTerminate.FormalFrameworks)
                    {
                        var organisationFormalFramework =
                            document
                                .FormalFrameworks
                                .Single(x => x.OrganisationFormalFrameworkId == key);

                        organisationFormalFramework.Validity.End = value;
                    }
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    foreach (var (key, value) in message.Body.FieldsToTerminate.FormalFrameworks)
                    {
                        var organisationFormalFramework =
                            document
                                .FormalFrameworks
                                .Single(x => x.OrganisationFormalFrameworkId == key);

                        organisationFormalFramework.Validity.End = value;
                    }
                }
            );
        }
    }
}
