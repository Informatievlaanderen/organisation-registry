namespace OrganisationRegistry.ElasticSearch.Projections.Body
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using App.Metrics;
    using App.Metrics.Timer;
    using Bodies;
    using Client;
    using Common;
    using Configuration;
    using Function.Events;
    using Infrastructure;
    using Infrastructure.Change;
    using LifecyclePhaseType.Events;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nest;
    using Organisation.Events;
    using Person.Events;
    using SeatType.Events;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer;
    using TimeUnit = App.Metrics.TimeUnit;

    public class BodyHandler :
        BaseProjection<BodyHandler>,
        IElasticEventHandler<InitialiseProjection>,
        IElasticEventHandler<BodyRegistered>,
        IElasticEventHandler<BodyInfoChanged>,
        IElasticEventHandler<BodyLifecyclePhaseAdded>,
        IElasticEventHandler<BodyLifecyclePhaseUpdated>,
        IElasticEventHandler<LifecyclePhaseTypeUpdated>,
        IElasticEventHandler<BodyFormalFrameworkAdded>,
        IElasticEventHandler<BodyFormalFrameworkUpdated>,
        IElasticEventHandler<BodyOrganisationAdded>,
        IElasticEventHandler<BodyOrganisationUpdated>,
        IElasticEventHandler<BodySeatAdded>,
        IElasticEventHandler<BodySeatUpdated>,
        IElasticEventHandler<AssignedPersonToBodySeat>,
        IElasticEventHandler<ReassignedPersonToBodySeat>,
        IElasticEventHandler<AssignedOrganisationToBodySeat>,
        IElasticEventHandler<ReassignedOrganisationToBodySeat>,
        IElasticEventHandler<AssignedFunctionTypeToBodySeat>,
        IElasticEventHandler<ReassignedFunctionTypeToBodySeat>,
        IElasticEventHandler<PersonAssignedToDelegation>,
        IElasticEventHandler<PersonAssignedToDelegationUpdated>,
        IElasticEventHandler<PersonAssignedToDelegationRemoved>,
        IElasticEventHandler<PersonUpdated>,
        IElasticEventHandler<OrganisationInfoUpdated>,
        IElasticEventHandler<OrganisationInfoUpdatedFromKbo>,
        IElasticEventHandler<OrganisationCouplingWithKboCancelled>,
        IElasticEventHandler<FunctionUpdated>,
        IElasticEventHandler<SeatTypeUpdated>
    {
        private readonly Elastic _elastic;
        private readonly IContextFactory _contextFactory;
        private readonly IMetricsRoot _metrics;
        private readonly ElasticSearchConfiguration _elasticSearchOptions;

        private static readonly TimeSpan ScrollTimeout = TimeSpan.FromMinutes(5);
        private readonly TimerOptions _indexTimer;

        private static IEnumerable<string> ProjectionTableNames => Array.Empty<string>();

        public BodyHandler(
            ILogger<BodyHandler> logger,
            Elastic elastic,
            IContextFactory contextFactory,
            IOptions<ElasticSearchConfiguration> elasticSearchOptions,
            IMetricsRoot metrics) : base(logger)
        {
            _elastic = elastic;
            _contextFactory = contextFactory;
            _metrics = metrics;
            _indexTimer = new TimerOptions
            {
                Name = "Index Refresh Timer",
                MeasurementUnit = Unit.Requests,
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds
            };
            _elasticSearchOptions = elasticSearchOptions.Value;
        }

        private void PrepareIndex(IElasticClient client, bool deleteIndex)
        {
            var indexName = _elasticSearchOptions.BodyWriteIndex;

            if (deleteIndex && client.DoesIndexExist(indexName))
            {
                var deleteResult = client.Indices.Delete(
                    new DeleteIndexRequest(Indices.Index(new List<IndexName> {indexName})));

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

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<InitialiseProjection> message)
        {
            if (message.Body.ProjectionName != typeof(BodyHandler).FullName)
                return new ElasticNoChange();

            Logger.LogInformation("Rebuilding index for {ProjectionName}.", message.Body.ProjectionName);
            PrepareIndex(_elastic.WriteClient, true);

            if (!ProjectionTableNames.Any())
                return new ElasticNoChange();

            await using var context = _contextFactory.Create();
            await context.Database.ExecuteSqlRawAsync(
                string.Concat(ProjectionTableNames.Select(tableName =>
                    $"DELETE FROM [ElasticSearchProjections].[{tableName}];")));

            return new ElasticNoChange();
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyRegistered> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
                    document.Id = message.Body.BodyId;
                    document.BodyNumber = message.Body.BodyNumber;
                    document.Name = message.Body.Name;
                    document.ShortName = message.Body.ShortName;
                    document.Description = message.Body.Description;
                    document.FormalValidity = new Period(
                        message.Body.FormalValidFrom,
                        message.Body.FormalValidTo);
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyInfoChanged> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
                    document.Name = message.Body.Name;
                    document.ShortName = message.Body.ShortName;
                    document.Description = message.Body.Description;
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyLifecyclePhaseAdded> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
                    document.LifecyclePhases.RemoveAndAdd(
                        x => x.BodyLifecyclePhaseId == message.Body.BodyLifecyclePhaseId,
                        new BodyDocument.BodyLifecyclePhase(
                            message.Body.BodyLifecyclePhaseId,
                            message.Body.LifecyclePhaseTypeId,
                            message.Body.LifecyclePhaseTypeName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyLifecyclePhaseUpdated> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
                    document.LifecyclePhases.RemoveAndAdd(
                        x => x.BodyLifecyclePhaseId == message.Body.BodyLifecyclePhaseId,
                        new BodyDocument.BodyLifecyclePhase(
                            message.Body.BodyLifecyclePhaseId,
                            message.Body.LifecyclePhaseTypeId,
                            message.Body.LifecyclePhaseTypeName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<LifecyclePhaseTypeUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic => elastic.Try(() => _elastic.WriteClient
                    .MassUpdateBody(
                        x => x.LifecyclePhases.Single().LifecyclePhaseTypeId, message.Body.LifecyclePhaseTypeId,
                        "lifecyclePhases", "lifecyclePhaseTypeId",
                        "lifecyclePhaseTypeName", message.Body.Name,
                        message.Number,
                        message.Timestamp))
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyFormalFrameworkAdded> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
                    document.FormalFrameworks.RemoveAndAdd(
                        x => x.BodyFormalFrameworkId == message.Body.BodyFormalFrameworkId,
                        new BodyDocument.BodyFormalFramework(
                            message.Body.BodyFormalFrameworkId,
                            message.Body.FormalFrameworkId,
                            message.Body.FormalFrameworkName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyFormalFrameworkUpdated> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
                    document.FormalFrameworks.RemoveAndAdd(
                        x => x.BodyFormalFrameworkId == message.Body.BodyFormalFrameworkId,
                        new BodyDocument.BodyFormalFramework(
                            message.Body.BodyFormalFrameworkId,
                            message.Body.FormalFrameworkId,
                            message.Body.FormalFrameworkName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyOrganisationAdded> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
                    document.Organisations.RemoveAndAdd(
                        x => x.BodyOrganisationId == message.Body.BodyOrganisationId,
                        new BodyDocument.BodyOrganisation(
                            message.Body.BodyOrganisationId,
                            message.Body.OrganisationId,
                            message.Body.OrganisationName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyOrganisationUpdated> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
                    document.Organisations.RemoveAndAdd(
                        x => x.BodyOrganisationId == message.Body.BodyOrganisationId,
                        new BodyDocument.BodyOrganisation(
                            message.Body.BodyOrganisationId,
                            message.Body.OrganisationId,
                            message.Body.OrganisationName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodySeatAdded> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
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
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodySeatUpdated> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
                    var bodySeat = document.Seats.Single(
                        x => x.BodySeatId == message.Body.BodySeatId);

                    bodySeat.Name = message.Body.Name;
                    bodySeat.PaidSeat = message.Body.PaidSeat;
                    bodySeat.EntitledToVote = message.Body.EntitledToVote;
                    bodySeat.SeatTypeId = message.Body.SeatTypeId;
                    bodySeat.SeatTypeName = message.Body.SeatTypeName;
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<AssignedPersonToBodySeat> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
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
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<ReassignedPersonToBodySeat> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
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
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<AssignedOrganisationToBodySeat> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
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
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<ReassignedOrganisationToBodySeat> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
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
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<AssignedFunctionTypeToBodySeat> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
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
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<ReassignedFunctionTypeToBodySeat> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
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
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<PersonAssignedToDelegation> message)
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var contactTypeNames = await organisationRegistryContext.ContactTypeList
                .Select(x => new {x.Id, x.Name})
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
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
                                    contactTypeNames[x.Key], x.Value)).ToList(),
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<PersonAssignedToDelegationUpdated> message)
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var contactTypeNames = await organisationRegistryContext.ContactTypeList
                .Select(x => new {x.Id, x.Name})
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
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
                                    contactTypeNames[x.Key], x.Value)).ToList(),
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<PersonAssignedToDelegationRemoved> message)
        {
            return new ElasticSingleChange
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
                    var bodySeat = document.Seats.Single(
                        x => x.BodySeatId == message.Body.BodySeatId);

                    var bodyMandate = bodySeat.Mandates.Single(
                        mandate => mandate.BodyMandateId == message.Body.BodyMandateId);

                    bodyMandate.Delegations.RemoveExistingListItems(
                        x => x.DelegationAssignmentId == message.Body.DelegationAssignmentId);
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<PersonUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic =>
                {
                    UpdatePersonForMandates(message, elastic);
                    UpdatePersonForDelegations(message, elastic);
                }
            );
        }

        private void UpdatePersonForDelegations(IEnvelope<PersonUpdated> message, Elastic elastic)
        {
            _metrics.Measure.Timer.Time(_indexTimer, () => elastic.WriteClient.Indices.Refresh(Indices.Index<BodyDocument>()));

            var searchResponse = elastic.WriteClient.Search<BodyDocument>(
                s => s.Query(
                    q => q.Term(
                        t => t
                            .Field(f => f.Seats.First().Mandates.First().Delegations.First().PersonId)
                            .Value(message.Body.PersonId))
                ).Scroll(new Time(ScrollTimeout)));

            var bodyDocuments = new List<BodyDocument>();

            while (searchResponse.Documents.Any())
            {
                searchResponse.Documents
                    .SelectMany(document => document.Seats)
                    .SelectMany(seat => seat.Mandates)
                    .SelectMany(seat => seat.Delegations)
                    .Where(delegation => delegation.PersonId == message.Body.PersonId)
                    .ToList()
                    .ForEach(delegation =>
                        delegation.PersonName = FormatPersonName(message.Body.FirstName, message.Body.Name));

                bodyDocuments.AddRange(searchResponse.Documents);

                searchResponse =
                    elastic.WriteClient.Scroll<BodyDocument>(new Time(ScrollTimeout), searchResponse.ScrollId);
            }
            elastic.WriteClient.ClearScroll(new ClearScrollRequest(searchResponse.ScrollId));

            if (!bodyDocuments.Any())
                return;

            elastic.Try(() => elastic.WriteClient.IndexMany(bodyDocuments));
        }

        private void UpdatePersonForMandates(IEnvelope<PersonUpdated> message, Elastic elastic)
        {
            _metrics.Measure.Timer.Time(_indexTimer, () => elastic.WriteClient.Indices.Refresh(Indices.Index<BodyDocument>()));

            var searchResponse = elastic.WriteClient.Search<BodyDocument>(
                s => s.Query(
                    q => q.Term(
                        t => t
                            .Field(f => f.Seats.Single().Mandates.Single().PersonId)
                            .Value(message.Body.PersonId))
                ).Scroll(new Time(ScrollTimeout)));

            var bodyDocuments = new List<BodyDocument>();

            while (searchResponse.Documents.Any())
            {
                searchResponse.Documents
                    .SelectMany(document => document.Seats)
                    .SelectMany(seat => seat.Mandates)
                    .Where(mandate => mandate.PersonId == message.Body.PersonId)
                    .ToList()
                    .ForEach(
                        mandate => mandate.PersonName = FormatPersonName(message.Body.FirstName, message.Body.Name));

                bodyDocuments.AddRange(searchResponse.Documents);

                searchResponse =
                    elastic.WriteClient.Scroll<BodyDocument>(new Time(ScrollTimeout), searchResponse.ScrollId);
            }
            elastic.WriteClient.ClearScroll(new ClearScrollRequest(searchResponse.ScrollId));

            if (!bodyDocuments.Any())
                return;

            elastic.Try(() => elastic.WriteClient.IndexMany(bodyDocuments));
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<OrganisationInfoUpdated> message)
        {
            return UpdateMandateOrganisationName(message.Body.OrganisationId, message.Body.Name);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            return UpdateMandateOrganisationName(message.Body.OrganisationId, message.Body.Name);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            return UpdateMandateOrganisationName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling);
        }

        private IElasticChange UpdateMandateOrganisationName(Guid organisationId, string organisationName)
        {
            return new ElasticMassChange
            (
                elastic =>
                {
                    _metrics.Measure.Timer.Time(_indexTimer, () => elastic.WriteClient.Indices.Refresh(Indices.Index<BodyDocument>()));

                    var searchResponse = elastic.WriteClient.Search<BodyDocument>(
                        s => s.Query(
                            q => q.Term(
                                t => t
                                    .Field(f => f.Seats.Single().Mandates.Single().OrganisationId)
                                    .Value(organisationId))
                        ).Scroll(new Time(ScrollTimeout)));

                    var bodyDocuments = new List<BodyDocument>();

                    while (searchResponse.Documents.Any())
                    {
                        searchResponse.Documents
                            .SelectMany(document => document.Seats)
                            .SelectMany(seat => seat.Mandates)
                            .Where(mandate => mandate.OrganisationId == organisationId)
                            .ToList()
                            .ForEach(mandate => mandate.OrganisationName = organisationName);

                        bodyDocuments.AddRange(searchResponse.Documents);

                        searchResponse = elastic.WriteClient.Scroll<BodyDocument>(new Time(ScrollTimeout), searchResponse.ScrollId);
                    }
                    elastic.WriteClient.ClearScroll(new ClearScrollRequest(searchResponse.ScrollId));

                    if (!bodyDocuments.Any())
                        return;

                    elastic.Try(() => elastic.WriteClient.IndexMany(bodyDocuments));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<FunctionUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic =>
                {
                    _metrics.Measure.Timer.Time(_indexTimer, () => elastic.WriteClient.Indices.Refresh(Indices.Index<BodyDocument>()));

                    var searchResponse = elastic.WriteClient.Search<BodyDocument>(
                        s => s.Query(
                            q => q.Term(
                                t => t
                                    .Field(f => f.Seats.Single().Mandates.Single().FunctionTypeId)
                                    .Value(message.Body.FunctionId))
                        ).Scroll(new Time(ScrollTimeout)));

                    var bodyDocuments = new List<BodyDocument>();

                    while (searchResponse.Documents.Any())
                    {
                        searchResponse.Documents
                            .SelectMany(document => document.Seats)
                            .SelectMany(seat => seat.Mandates)
                            .Where(mandate => mandate.FunctionTypeId == message.Body.FunctionId)
                            .ToList()
                            .ForEach(mandate => mandate.FunctionTypeName = message.Body.Name);

                        bodyDocuments.AddRange(searchResponse.Documents);

                        searchResponse = elastic.WriteClient.Scroll<BodyDocument>(new Time(ScrollTimeout), searchResponse.ScrollId);
                    }
                    elastic.WriteClient.ClearScroll(new ClearScrollRequest(searchResponse.ScrollId));

                    if (!bodyDocuments.Any())
                        return;

                    elastic.Try(() => elastic.WriteClient.IndexMany(bodyDocuments));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<SeatTypeUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic => elastic.WriteClient
                    .MassUpdateBody(
                        x => x.Seats.Single().SeatTypeId, message.Body.SeatTypeId,
                        "seats", "seatTypeId",
                        "seatTypeName", message.Body.Name,
                        message.Number,
                        message.Timestamp)
            );
        }
    }
}
