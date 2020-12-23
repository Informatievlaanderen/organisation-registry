namespace OrganisationRegistry.ElasticSearch.Projections.Body
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using Bodies;
    using Client;
    using Common;
    using Configuration;
    using Function.Events;
    using Infrastructure;
    using LifecyclePhaseType.Events;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nest;
    using Organisation.Events;
    using Person.Events;
    using SeatType.Events;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Events;

    public class BodyHandler :
        Infrastructure.BaseProjection<BodyHandler>,
        IEventHandler<InitialiseProjection>,
        IEventHandler<BodyRegistered>,
        IEventHandler<BodyInfoChanged>,
        IEventHandler<BodyLifecyclePhaseAdded>,
        IEventHandler<BodyLifecyclePhaseUpdated>,
        IEventHandler<LifecyclePhaseTypeUpdated>,
        IEventHandler<BodyFormalFrameworkAdded>,
        IEventHandler<BodyFormalFrameworkUpdated>,
        IEventHandler<BodyOrganisationAdded>,
        IEventHandler<BodyOrganisationUpdated>,
        IEventHandler<BodySeatAdded>,
        IEventHandler<BodySeatUpdated>,
        IEventHandler<AssignedPersonToBodySeat>,
        IEventHandler<ReassignedPersonToBodySeat>,
        IEventHandler<AssignedOrganisationToBodySeat>,
        IEventHandler<ReassignedOrganisationToBodySeat>,
        IEventHandler<AssignedFunctionTypeToBodySeat>,
        IEventHandler<ReassignedFunctionTypeToBodySeat>,
        IEventHandler<PersonAssignedToDelegation>,
        IEventHandler<PersonAssignedToDelegationUpdated>,
        IEventHandler<PersonAssignedToDelegationRemoved>,
        IEventHandler<PersonUpdated>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
        IEventHandler<FunctionUpdated>,
        IEventHandler<SeatTypeUpdated>
    {
        private readonly Elastic _elastic;
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;
        private readonly ElasticSearchConfiguration _elasticSearchOptions;
        private readonly IMemoryCaches _memoryCaches;
        private readonly ElasticWriter<BodyDocument> _elasticWriter;

        private static string[] ProjectionTableNames => Array.Empty<string>();

        public BodyHandler(
            ILogger<BodyHandler> logger,
            Elastic elastic,
            Func<Owned<OrganisationRegistryContext>> contextFactory,
            IOptions<ElasticSearchConfiguration> elasticSearchOptions,
            IMemoryCaches memoryCaches) : base(logger)
        {
            _elastic = elastic;
            _contextFactory = contextFactory;
            _memoryCaches = memoryCaches;
            _elasticSearchOptions = elasticSearchOptions.Value;

            _elasticWriter = new ElasticWriter<BodyDocument>(elastic);

            PrepareIndex(elastic.WriteClient, false);
        }

        private void PrepareIndex(IElasticClient client, bool deleteIndex)
        {
            var indexName = _elasticSearchOptions.BodyWriteIndex;

            if (deleteIndex && client.DoesIndexExist(indexName))
            {
                var deleteResult = client.Indices.Delete(
                    new DeleteIndexRequest(Indices.Index(new List<IndexName> { indexName })));

                if (!deleteResult.IsValid)
                    throw new Exception($"Could not delete body index '{indexName}'.");
            }

            if (!client.DoesIndexExist(indexName))
            {
                var indexResult = client.Indices.Create(
                    indexName,
                    index => index
                        .Map<BodyDocument>(BodyDocument.Mapping)
                        .Settings(descriptor => descriptor
                            .NumberOfShards(_elasticSearchOptions.NumberOfShards)
                            .NumberOfReplicas(_elasticSearchOptions.NumberOfReplicas)));

                if (!indexResult.IsValid)
                    throw new Exception($"Could not create body index '{indexName}'.");
            }
        }

        private static string FormatPersonName(string firstName, string name)
        {
            return $"{firstName} {name}";
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<InitialiseProjection> message)
        {
            if (message.Body.ProjectionName != typeof(BodyHandler).FullName)
                return;

            Logger.LogInformation("Rebuilding index for {ProjectionName}.", message.Body.ProjectionName);
            PrepareIndex(_elastic.WriteClient, true);

            if (!ProjectionTableNames.Any())
                return;

            using (var context = _contextFactory().Value)
                context.Database.ExecuteSqlRaw(
                    string.Concat(ProjectionTableNames.Select(tableName => $"DELETE FROM [OrganisationRegistry].[{tableName}];")));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyRegistered> message)
        {
            _elasticWriter
                .Create(message)
                .CommitDocument(document =>
                {
                    document.Id = message.Body.BodyId;
                    document.BodyNumber = message.Body.BodyNumber;
                    document.Name = message.Body.Name;
                    document.ShortName = message.Body.ShortName;
                    document.Description = message.Body.Description;
                    document.FormalValidity = new Period(
                        message.Body.FormalValidFrom,
                        message.Body.FormalValidTo);
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyInfoChanged> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    document.Name = message.Body.Name;
                    document.ShortName = message.Body.ShortName;
                    document.Description = message.Body.Description;
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyLifecyclePhaseAdded> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    document.LifecyclePhases.RemoveAndAdd(
                        x => x.BodyLifecyclePhaseId == message.Body.BodyLifecyclePhaseId,
                        new BodyDocument.BodyLifecyclePhase(
                            message.Body.BodyLifecyclePhaseId,
                            message.Body.LifecyclePhaseTypeId,
                            message.Body.LifecyclePhaseTypeName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyLifecyclePhaseUpdated> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    document.LifecyclePhases.RemoveAndAdd(
                        x => x.BodyLifecyclePhaseId == message.Body.BodyLifecyclePhaseId,
                        new BodyDocument.BodyLifecyclePhase(
                            message.Body.BodyLifecyclePhaseId,
                            message.Body.LifecyclePhaseTypeId,
                            message.Body.LifecyclePhaseTypeName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LifecyclePhaseTypeUpdated> message)
        {
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateBody(
                    x => x.LifecyclePhases.Single().LifecyclePhaseTypeId, message.Body.LifecyclePhaseTypeId,
                    "lifecyclePhases", "lifecyclePhaseTypeId",
                    "lifecyclePhaseTypeName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyFormalFrameworkAdded> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    document.FormalFrameworks.RemoveAndAdd(
                        x => x.BodyFormalFrameworkId == message.Body.BodyFormalFrameworkId,
                        new BodyDocument.BodyFormalFramework(
                            message.Body.BodyFormalFrameworkId,
                            message.Body.FormalFrameworkId,
                            message.Body.FormalFrameworkName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyFormalFrameworkUpdated> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    document.FormalFrameworks.RemoveAndAdd(
                        x => x.BodyFormalFrameworkId == message.Body.BodyFormalFrameworkId,
                        new BodyDocument.BodyFormalFramework(
                            message.Body.BodyFormalFrameworkId,
                            message.Body.FormalFrameworkId,
                            message.Body.FormalFrameworkName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationAdded> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    document.Organisations.RemoveAndAdd(
                        x => x.BodyOrganisationId == message.Body.BodyOrganisationId,
                        new BodyDocument.BodyOrganisation(
                            message.Body.BodyOrganisationId,
                            message.Body.OrganisationId,
                            message.Body.OrganisationName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    document.Organisations.RemoveAndAdd(
                        x => x.BodyOrganisationId == message.Body.BodyOrganisationId,
                        new BodyDocument.BodyOrganisation(
                            message.Body.BodyOrganisationId,
                            message.Body.OrganisationId,
                            message.Body.OrganisationName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatAdded> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    document.Seats.RemoveAndAdd(
                        x => x.BodySeatId == message.Body.BodySeatId,
                        new BodyDocument.BodySeat(
                            message.Body.BodySeatId,
                            message.Body.BodySeatNumber,
                            message.Body.Name,
                            message.Body.PaidSeat,
                            message.Body.EntitledToVote,
                            message.Body.SeatTypeId,
                            message.Body.SeatTypeName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatUpdated> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    var bodySeat = document.Seats.Single(
                        x => x.BodySeatId == message.Body.BodySeatId);

                    bodySeat.Name = message.Body.Name;
                    bodySeat.PaidSeat = message.Body.PaidSeat;
                    bodySeat.EntitledToVote = message.Body.EntitledToVote;
                    bodySeat.SeatTypeId = message.Body.SeatTypeId;
                    bodySeat.SeatTypeName = message.Body.SeatTypeName;
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedPersonToBodySeat> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    var bodySeat = document.Seats.Single(
                        x => x.BodySeatId == message.Body.BodySeatId);

                    bodySeat.Mandates.RemoveAndAdd(
                        x => x.BodyMandateId == message.Body.BodyMandateId,
                        new BodyDocument.BodyMandate(
                            message.Body.BodyMandateId,
                            null,
                            null,
                            null,
                            null,
                            message.Body.PersonId,
                            FormatPersonName(message.Body.PersonFirstName, message.Body.PersonName),
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedPersonToBodySeat> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    var bodySeat = document.Seats.Single(
                        x => x.BodySeatId == message.Body.BodySeatId);

                    bodySeat.Mandates.RemoveAndAdd(
                        x => x.BodyMandateId == message.Body.BodyMandateId,
                        new BodyDocument.BodyMandate(
                            message.Body.BodyMandateId,
                            null,
                            null,
                            null,
                            null,
                            message.Body.PersonId,
                            FormatPersonName(message.Body.PersonFirstName, message.Body.PersonName),
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedOrganisationToBodySeat> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    var bodySeat = document.Seats.Single(
                        x => x.BodySeatId == message.Body.BodySeatId);

                    bodySeat.Mandates.RemoveAndAdd(
                        x => x.BodyMandateId == message.Body.BodyMandateId,
                        new BodyDocument.BodyMandate(
                            message.Body.BodyMandateId,
                            message.Body.OrganisationId,
                            message.Body.OrganisationName,
                            null,
                            null,
                            null,
                            null,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedOrganisationToBodySeat> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    var bodySeat = document.Seats.Single(
                        x => x.BodySeatId == message.Body.BodySeatId);

                    bodySeat.Mandates.RemoveAndAdd(
                        x => x.BodyMandateId == message.Body.BodyMandateId,
                        new BodyDocument.BodyMandate(
                            message.Body.BodyMandateId,
                            message.Body.OrganisationId,
                            message.Body.OrganisationName,
                            null,
                            null,
                            null,
                            null,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedFunctionTypeToBodySeat> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    var bodySeat = document.Seats.Single(
                        x => x.BodySeatId == message.Body.BodySeatId);

                    bodySeat.Mandates.RemoveAndAdd(
                        x => x.BodyMandateId == message.Body.BodyMandateId,
                        new BodyDocument.BodyMandate(
                            message.Body.BodyMandateId,
                            message.Body.OrganisationId,
                            message.Body.OrganisationName,
                            message.Body.FunctionTypeId,
                            message.Body.FunctionTypeName,
                            null,
                            null,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedFunctionTypeToBodySeat> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    var bodySeat = document.Seats.Single(
                        x => x.BodySeatId == message.Body.BodySeatId);

                    bodySeat.Mandates.RemoveAndAdd(
                        x => x.BodyMandateId == message.Body.BodyMandateId,
                        new BodyDocument.BodyMandate(
                            message.Body.BodyMandateId,
                            message.Body.OrganisationId,
                            message.Body.OrganisationName,
                            message.Body.FunctionTypeId,
                            message.Body.FunctionTypeName,
                            null,
                            null,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegation> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    var bodySeat = document.Seats.Single(
                        x => x.BodySeatId == message.Body.BodySeatId);

                    var bodyMandate = bodySeat.Mandates.Single(
                        mandate => mandate.BodyMandateId == message.Body.BodyMandateId);

                    bodyMandate.Delegations.RemoveAndAdd(
                        x => x.DelegationAssignmentId == message.Body.DelegationAssignmentId,
                        new BodyDocument.Delegation(
                            message.Body.DelegationAssignmentId,
                            message.Body.PersonId,
                            message.Body.PersonFullName,
                            message.Body.Contacts.Select(x =>
                                new Contact(
                                    x.Key,
                                    _memoryCaches.ContactTypeNames[x.Key], x.Value)).ToList(),
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationUpdated> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    var bodySeat = document.Seats.Single(
                        x => x.BodySeatId == message.Body.BodySeatId);

                    var bodyMandate = bodySeat.Mandates.Single(
                        mandate => mandate.BodyMandateId == message.Body.BodyMandateId);

                    bodyMandate.Delegations.RemoveAndAdd(
                        x => x.DelegationAssignmentId == message.Body.DelegationAssignmentId,
                        new BodyDocument.Delegation(
                            message.Body.DelegationAssignmentId,
                            message.Body.PersonId,
                            message.Body.PersonFullName,
                            message.Body.Contacts.Select(x =>
                                new Contact(
                                    x.Key,
                                    _memoryCaches.ContactTypeNames[x.Key], x.Value)).ToList(),
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationRemoved> message)
        {
            _elasticWriter
                .Update(message.Body.BodyId, message)
                .CommitDocument(document =>
                {
                    var bodySeat = document.Seats.Single(
                        x => x.BodySeatId == message.Body.BodySeatId);

                    var bodyMandate = bodySeat.Mandates.Single(
                        mandate => mandate.BodyMandateId == message.Body.BodyMandateId);

                    bodyMandate.Delegations.RemoveExistingListItems(
                        x => x.DelegationAssignmentId == message.Body.DelegationAssignmentId);
                });
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonUpdated> message)
        {
            UpdatePersonForMandates(message);
            UpdatePersonForDelegations(message);
        }

        private void UpdatePersonForMandates(IEnvelope<PersonUpdated> message)
        {
            // TODO: we will need to move away from search, since that is limited in results
            _elastic.WriteClient.Indices.Refresh(Indices.All);

            var results =
                _elastic.TryGet(
                    () => _elastic.WriteClient.Search<BodyDocument>(
                            s => s.Query(
                                q => q.Term(
                                    t => t
                                        .Field(f => f.Seats.Single().Mandates.Single().PersonId)
                                        .Value(message.Body.PersonId))
                            ))
                        .ThrowOnFailure());

            var bodyDocuments = results.Documents;

            if (!bodyDocuments.Any())
                return;

            bodyDocuments
                .SelectMany(document => document.Seats)
                .SelectMany(seat => seat.Mandates)
                .Where(mandate => mandate.PersonId == message.Body.PersonId)
                .ToList()
                .ForEach(
                    mandate => mandate.PersonName = FormatPersonName(message.Body.FirstName, message.Body.Name));

            _elastic.Try(() => _elastic.WriteClient.IndexMany(bodyDocuments).ThrowOnFailure());
        }

        private void UpdatePersonForDelegations(IEnvelope<PersonUpdated> message)
        {
            _elastic.WriteClient.Indices.Refresh(Indices.All);

            var delegationResults =
                _elastic.TryGet(
                    () => _elastic.WriteClient.Search<BodyDocument>(
                            s => s.Query(
                                q => q.Term(
                                    t => t
                                        .Field(f => f.Seats.First().Mandates.First().Delegations.First().PersonId)
                                        .Value(message.Body.PersonId))
                            ))
                        .ThrowOnFailure());

            var bodyDocumentsForDelegations = delegationResults.Documents;

            if (!bodyDocumentsForDelegations.Any())
                return;

            bodyDocumentsForDelegations
                .SelectMany(document => document.Seats)
                .SelectMany(seat => seat.Mandates)
                .SelectMany(seat => seat.Delegations)
                .Where(delegation => delegation.PersonId == message.Body.PersonId)
                .ToList()
                .ForEach(delegation => delegation.PersonName = FormatPersonName(message.Body.FirstName, message.Body.Name));

            _elastic.Try(() => _elastic.WriteClient.IndexMany(bodyDocumentsForDelegations).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            UpdateMandateOrganisationName(message.Body.OrganisationId, message.Body.Name);

        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            UpdateMandateOrganisationName(message.Body.OrganisationId, message.Body.Name);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            UpdateMandateOrganisationName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling);
        }

        private void UpdateMandateOrganisationName(Guid organisationId, string organisationName)
        {
            _elastic.WriteClient.Indices.Refresh(Indices.All);
            var results =
                _elastic.TryGet(
                    () => _elastic.WriteClient.Search<BodyDocument>(
                            s => s.Query(
                                q => q.Term(
                                    t => t
                                        .Field(f => f.Seats.Single().Mandates.Single().OrganisationId)
                                        .Value(organisationId))
                            ))
                        .ThrowOnFailure());

            var bodyDocuments = results.Documents;

            if (!bodyDocuments.Any())
                return;

            bodyDocuments
                .SelectMany(document => document.Seats)
                .SelectMany(seat => seat.Mandates)
                .Where(mandate => mandate.OrganisationId == organisationId)
                .ToList()
                .ForEach(mandate => mandate.OrganisationName = organisationName);

            _elastic.Try(() => _elastic.WriteClient.IndexMany(bodyDocuments).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            _elastic.WriteClient.Indices.Refresh(Indices.All);
            var results =
                _elastic.TryGet(
                    () => _elastic.WriteClient.Search<BodyDocument>(
                            s => s.Query(
                                q => q.Term(
                                    t => t
                                        .Field(f => f.Seats.Single().Mandates.Single().FunctionTypeId)
                                        .Value(message.Body.FunctionId))
                            ))
                        .ThrowOnFailure());

            var bodyDocuments = results.Documents;

            if (!bodyDocuments.Any())
                return;

            bodyDocuments
                .SelectMany(document => document.Seats)
                .SelectMany(seat => seat.Mandates)
                .Where(mandate => mandate.FunctionTypeId == message.Body.FunctionId)
                .ToList()
                .ForEach(mandate => mandate.FunctionTypeName = message.Body.Name);

            _elastic.Try(() => _elastic.WriteClient.IndexMany(bodyDocuments).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<SeatTypeUpdated> message)
        {
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateBody(
                    x => x.Seats.Single().SeatTypeId, message.Body.SeatTypeId,
                    "seats", "seatTypeId",
                    "seatTypeName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }
    }

    public class ElasticWriter<T> where T : class, IDocument, new()
    {
        private readonly Elastic _elastic;

        public ElasticWriter(Elastic elastic)
        {
            _elastic = elastic;
        }

        public DocumentBuilder<T> Create(IEnvelope envelope)
        {
            return new DocumentBuilder<T>(_elastic, envelope.Number, envelope.Timestamp, elastic => new T());
        }

        public DocumentBuilder<T> Update(Guid id, IEnvelope envelope)
        {
            return new DocumentBuilder<T>(
                _elastic,
                envelope.Number,
                envelope.Timestamp,
                elastic => elastic.TryGet(() => elastic.WriteClient.Get<T>(id).ThrowOnFailure().Source));
        }
    }

    public class DocumentBuilder<T> where T : class, IDocument
    {
        private readonly Elastic _elastic;
        private readonly int _changeId;
        private readonly DateTimeOffset _changeTime;
        private readonly Func<Elastic, T> _documentCreateFunc;

        public DocumentBuilder(Elastic elastic, int changeId, DateTimeOffset changeTime, Func<Elastic, T> documentCreateFunc)
        {
            _elastic = elastic;

            _changeId = changeId;
            _changeTime = changeTime;

            _documentCreateFunc = documentCreateFunc;
        }

        public void CommitDocument(Action<T> updateDocument)
        {
            var document = _documentCreateFunc(_elastic);

            updateDocument(document);

            document.ChangeId = _changeId;
            document.ChangeTime = _changeTime;

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(document).ThrowOnFailure());
        }
    }
}
