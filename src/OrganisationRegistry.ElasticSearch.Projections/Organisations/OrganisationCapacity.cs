namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using Capacity.Events;
    using Client;
    using ElasticSearch.Organisations;
    using Function.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Person.Events;
    using Common;
    using Infrastructure.Change;
    using Microsoft.EntityFrameworkCore;
    using SqlServer;

    public class OrganisationCapacity :
        BaseProjection<OrganisationCapacity>,
        IElasticEventHandler<OrganisationCapacityAdded>,
        IElasticEventHandler<OrganisationCapacityUpdated>,
        IElasticEventHandler<CapacityUpdated>,
        IElasticEventHandler<FunctionUpdated>,
        IElasticEventHandler<PersonUpdated>,
        IElasticEventHandler<OrganisationTerminated>
    {
        private readonly IContextFactory _contextFactory;

        public OrganisationCapacity(
            ILogger<OrganisationCapacity> logger,
            IContextFactory contextFactory) : base(logger)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<CapacityUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic.WriteClient
                    .MassUpdateOrganisationAsync(
                        x => x.Capacities.Single().CapacityId, message.Body.CapacityId,
                        "capacities", "capacityId",
                        "capacityName", message.Body.Name,
                        message.Number,
                        message.Timestamp))
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic.WriteClient
                    .MassUpdateOrganisationAsync(
                        x => x.Capacities.Single().FunctionId, message.Body.FunctionId,
                        "capacities", "functionId",
                        "functionName", message.Body.Name,
                        message.Number,
                        message.Timestamp))
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic.WriteClient
                    .MassUpdateOrganisationAsync(
                        x => x.Capacities.Single().PersonId, message.Body.PersonId,
                        "capacities", "personId",
                        "personName", $"{message.Body.Name} {message.Body.FirstName}",
                        message.Number,
                        message.Timestamp))
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityAdded> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    await using var organisationRegistryContext = _contextFactory.Create();
                    var contactTypeNames = await organisationRegistryContext.ContactTypeList
                        .Select(x => new {x.Id, x.Name})
                        .ToDictionaryAsync(x => x.Id, x => x.Name);

                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.Capacities == null)
                        document.Capacities = new List<OrganisationDocument.OrganisationCapacity>();

                    document.Capacities.RemoveExistingListItems(x => x.OrganisationCapacityId == message.Body.OrganisationCapacityId);

                    document.Capacities.Add(
                        new OrganisationDocument.OrganisationCapacity(
                            message.Body.OrganisationCapacityId,
                            message.Body.CapacityId,
                            message.Body.CapacityName,
                            message.Body.PersonId,
                            message.Body.PersonFullName,
                            message.Body.FunctionId,
                            message.Body.FunctionName,
                            message.Body.Contacts.Select(x => new Contact(x.Key, contactTypeNames[x.Key], x.Value)).ToList(),
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityUpdated> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    await using var organisationRegistryContext = _contextFactory.Create();
                    var contactTypeNames = await organisationRegistryContext.ContactTypeList
                        .Select(x => new {x.Id, x.Name})
                        .ToDictionaryAsync(x => x.Id, x => x.Name);

                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Capacities.RemoveExistingListItems(x => x.OrganisationCapacityId == message.Body.OrganisationCapacityId);

                    document.Capacities.Add(
                        new OrganisationDocument.OrganisationCapacity(
                            message.Body.OrganisationCapacityId,
                            message.Body.CapacityId,
                            message.Body.CapacityName,
                            message.Body.PersonId,
                            message.Body.PersonFullName,
                            message.Body.FunctionId,
                            message.Body.FunctionName,
                            message.Body.Contacts.Select(x => new Contact(x.Key, contactTypeNames[x.Key], x.Value)).ToList(),
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

                    foreach (var (key, value) in message.Body.FieldsToTerminate.Capacities)
                    {
                        var organisationCapacity =
                            document
                                .Capacities
                                .Single(x => x.OrganisationCapacityId == key);

                        organisationCapacity.Validity.End = value;
                    }
                }
            );
        }
    }
}
