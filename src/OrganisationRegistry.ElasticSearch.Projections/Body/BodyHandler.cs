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
    using Osc;
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
        IElasticEventHandler<BodyNumberAssigned>,
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
        IElasticEventHandler<OrganisationNameUpdated>,
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

        private static string[] ProjectionTableNames => Array.Empty<string>();

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

        private async Task PrepareIndex(IOpenSearchClient client, bool deleteIndex)
        {
            var indexName = _elasticSearchOptions.BodyWriteIndex;

            if (deleteIndex && await client.DoesIndexExist(indexName))
            {
                var deleteResult = await client.Indices.DeleteAsync(
                    new DeleteIndexRequest(Indices.Index(new List<IndexName> {indexName})));

                if (!deleteResult.IsValid)
                    throw new Exception($"Could not delete body index '{indexName}'.");
            }

            if (!await client.DoesIndexExist(indexName))
            {
                var indexResult = await client.Indices.CreateAsync(
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
            await PrepareIndex(_elastic.WriteClient, true);

            if (!ProjectionTableNames.Any())
                return new ElasticNoChange();

            await using var context = _contextFactory.Create();
            await context.Database.DeleteAllRows(
                OrganisationRegistry.Infrastructure.WellknownSchemas.ElasticSearchProjectionsSchema,
                ProjectionTableNames);
            return new ElasticNoChange();
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyRegistered> message)
        {
            return new ElasticDocumentCreation<BodyDocument>
            (
                message.Body.BodyId,
                () => new BodyDocument
                {
                    ChangeId = message.Number,
                    ChangeTime = message.Timestamp,
                    Id = message.Body.BodyId,
                    BodyNumber = message.Body.BodyNumber,
                    Name = message.Body.Name,
                    ShortName = message.Body.ShortName,
                    Description = message.Body.Description,
                    FormalValidity = Period.FromDates(message.Body.FormalValidFrom,
                        message.Body.FormalValidTo)
                });
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyInfoChanged> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyNumberAssigned> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
            (
                message.Body.BodyId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;
                    document.BodyNumber = message.Body.BodyNumber;
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyLifecyclePhaseAdded> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyLifecyclePhaseUpdated> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<LifecyclePhaseTypeUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic
                    .MassUpdateBodyAsync(
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
            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyFormalFrameworkUpdated> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyOrganisationAdded> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyOrganisationUpdated> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodySeatAdded> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodySeatUpdated> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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
                    bodySeat.Validity = Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo);
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<AssignedPersonToBodySeat> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<ReassignedPersonToBodySeat> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<AssignedOrganisationToBodySeat> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<ReassignedOrganisationToBodySeat> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<AssignedFunctionTypeToBodySeat> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<ReassignedFunctionTypeToBodySeat> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<PersonAssignedToDelegation> message)
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var contactTypeNames = await organisationRegistryContext.ContactTypeCache
                .Select(x => new {x.Id, x.Name})
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<PersonAssignedToDelegationUpdated> message)
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var contactTypeNames = await organisationRegistryContext.ContactTypeCache
                .Select(x => new {x.Id, x.Name})
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            return new ElasticPerDocumentChange<BodyDocument>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<PersonAssignedToDelegationRemoved> message)
        {
            return new ElasticPerDocumentChange<BodyDocument>
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
            return new ElasticMassChange(
                async elastic =>
                {
                    await UpdatePersonForMandates(message, elastic);
                    await UpdatePersonForDelegations(message, elastic);
                }
            );
        }

        private async Task UpdatePersonForDelegations(IEnvelope<PersonUpdated> message, Elastic elastic)
        {

            await elastic.TryGetAsync(async () =>
                (await elastic.WriteClient.Indices.RefreshAsync(Indices.Index<BodyDocument>())).ThrowOnFailure());

            var searchResponse = (await elastic.TryGetAsync(() => elastic.WriteClient.SearchAsync<BodyDocument>(
                s => s.Query(
                    q => q.Term(
                        t => t
                            .Field(f => f.Seats.First().Mandates.First().Delegations.First().PersonId)
                            .Value(message.Body.PersonId))
                ).Scroll(new Time(ScrollTimeout))))).ThrowOnFailure();

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

                var response = searchResponse;
                searchResponse = (await elastic.TryGetAsync(() =>
                    elastic.WriteClient.ScrollAsync<BodyDocument>(new Time(ScrollTimeout),
                        response.ScrollId))).ThrowOnFailure();
            }
            (await elastic.TryGetAsync(() => elastic.WriteClient.ClearScrollAsync(new ClearScrollRequest(searchResponse.ScrollId)))).ThrowOnFailure();

            if (!bodyDocuments.Any())
                return;

            elastic.WriteClient.BulkAll(bodyDocuments, b => b
                .BackOffTime("30s")
                .BackOffRetries(5)
                .RefreshOnCompleted(false)
                .MaxDegreeOfParallelism(Environment.ProcessorCount)
                .Size(1000)
            )
            .Wait(TimeSpan.FromMinutes(15), next =>
            {
                Logger.LogInformation("Writing page {PageNumber}", next.Page);
            });
        }

        private async Task UpdatePersonForMandates(IEnvelope<PersonUpdated> message, Elastic elastic)
        {
            await elastic.TryGetAsync(async () =>
                (await elastic.WriteClient.Indices.RefreshAsync(Indices.Index<BodyDocument>())).ThrowOnFailure());

            var searchResponse = (await elastic.TryGetAsync(() => elastic.WriteClient.SearchAsync<BodyDocument>(
                s => s.Query(
                    q => q.Term(
                        t => t
                            .Field(f => f.Seats.Single().Mandates.Single().PersonId)
                            .Value(message.Body.PersonId))
                ).Scroll(new Time(ScrollTimeout))))).ThrowOnFailure();

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

                var response = searchResponse;
                searchResponse = (await elastic.TryGetAsync(() =>
                    elastic.WriteClient.ScrollAsync<BodyDocument>(new Time(ScrollTimeout),
                        response.ScrollId))).ThrowOnFailure();
            }
            (await elastic.TryGetAsync(() => elastic.WriteClient.ClearScrollAsync(new ClearScrollRequest(searchResponse.ScrollId)))).ThrowOnFailure();

            if (!bodyDocuments.Any())
                return;

            elastic.WriteClient.BulkAll(bodyDocuments, b => b
                .BackOffTime("30s")
                .BackOffRetries(5)
                .RefreshOnCompleted(false)
                .MaxDegreeOfParallelism(Environment.ProcessorCount)
                .Size(1000)
            )
            .Wait(TimeSpan.FromMinutes(15), next =>
            {
                Logger.LogInformation("Writing page {PageNumber}", next.Page);
            });
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<OrganisationInfoUpdated> message)
        {
            return UpdateMandateOrganisationName(message.Body.OrganisationId, message.Body.Name);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<OrganisationNameUpdated> message)
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
                async elastic =>
                {
                    using (_metrics.Measure.Timer.Time(_indexTimer))
                    {
                        await elastic.TryGetAsync(async () =>
                            (await elastic.WriteClient.Indices.RefreshAsync(Indices.Index<BodyDocument>())).ThrowOnFailure());
                    }

                    var searchResponse = (await elastic.TryGetAsync(() => elastic.WriteClient.SearchAsync<BodyDocument>(
                        s => s.Query(
                            q => q.Term(
                                t => t
                                    .Field(f => f.Seats.Single().Mandates.Single().OrganisationId)
                                    .Value(organisationId))
                        ).Scroll(new Time(ScrollTimeout))))).ThrowOnFailure();

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

                        var response = searchResponse;
                        searchResponse = (await elastic.TryGetAsync(() =>
                            elastic.WriteClient.ScrollAsync<BodyDocument>(new Time(ScrollTimeout),
                                response.ScrollId))).ThrowOnFailure();
                    }
                    (await elastic.TryGetAsync(() => elastic.WriteClient.ClearScrollAsync(new ClearScrollRequest(searchResponse.ScrollId)))).ThrowOnFailure();

                    if (!bodyDocuments.Any())
                        return;

                    elastic.WriteClient.BulkAll(bodyDocuments, b => b
                        .BackOffTime("30s")
                        .BackOffRetries(5)
                        .RefreshOnCompleted(false)
                        .MaxDegreeOfParallelism(Environment.ProcessorCount)
                        .Size(1000)
                    )
                    .Wait(TimeSpan.FromMinutes(15), next =>
                    {
                        Logger.LogInformation("Writing page {PageNumber}", next.Page);
                    });
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<FunctionUpdated> message)
        {
            return new ElasticMassChange
            (
                async elastic =>
                {
                    using (_metrics.Measure.Timer.Time(_indexTimer))
                    {
                        await elastic.TryGetAsync(async () =>
                            (await elastic.WriteClient.Indices.RefreshAsync(Indices.Index<BodyDocument>()))
                            .ThrowOnFailure());
                    }

                    var searchResponse = (await elastic.TryGetAsync(() => elastic.WriteClient.SearchAsync<BodyDocument>(
                        s => s.Query(
                            q => q.Term(
                                t => t
                                    .Field(f => f.Seats.Single().Mandates.Single().FunctionTypeId)
                                    .Value(message.Body.FunctionId))
                        ).Scroll(new Time(ScrollTimeout))))).ThrowOnFailure();

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

                        var response = searchResponse;
                        searchResponse = (await elastic.TryGetAsync(() =>
                            elastic.WriteClient.ScrollAsync<BodyDocument>(new Time(ScrollTimeout),
                                response.ScrollId))).ThrowOnFailure();
                    }

                    (await elastic.TryGetAsync(() =>
                            elastic.WriteClient.ClearScrollAsync(new ClearScrollRequest(searchResponse.ScrollId))))
                        .ThrowOnFailure();

                    if (!bodyDocuments.Any())
                        return;

                    elastic.WriteClient.BulkAll(bodyDocuments, b => b
                        .BackOffTime("30s")
                        .BackOffRetries(5)
                        .RefreshOnCompleted(false)
                        .MaxDegreeOfParallelism(Environment.ProcessorCount)
                        .Size(1000)
                    )
                    .Wait(TimeSpan.FromMinutes(15),
                        next =>
                        {
                            Logger.LogInformation("Writing page {PageNumber}", next.Page);
                        });
                });
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<SeatTypeUpdated> message)
        {
            return new ElasticMassChange
            (
                async elastic => await elastic
                    .MassUpdateBodyAsync(
                        x => x.Seats.Single().SeatTypeId, message.Body.SeatTypeId,
                        "seats", "seatTypeId",
                        "seatTypeName", message.Body.Name,
                        message.Number,
                        message.Timestamp)
            );
        }
    }
}
