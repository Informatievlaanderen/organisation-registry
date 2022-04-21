namespace OrganisationRegistry.ElasticSearch.Projections.People.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Cache;
    using Common;
    using ElasticSearch.People;
    using Infrastructure;
    using Infrastructure.Change;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Organisation.Events;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer;

    public class PersonMandate :
        Infrastructure.BaseProjection<PersonMandate>,
        IElasticEventHandler<AssignedPersonToBodySeat>,
        IElasticEventHandler<ReassignedPersonToBodySeat>,
        IElasticEventHandler<BodyInfoChanged>,
        IElasticEventHandler<BodyOrganisationUpdated>,
        IElasticEventHandler<BodySeatUpdated>,
        IElasticEventHandler<OrganisationInfoUpdated>,
        IElasticEventHandler<OrganisationNameUpdated>,
        IElasticEventHandler<OrganisationInfoUpdatedFromKbo>,
        IElasticEventHandler<OrganisationCouplingWithKboCancelled>,
        IElasticEventHandler<PersonAssignedToDelegation>,
        IElasticEventHandler<PersonAssignedToDelegationUpdated>,
        IElasticEventHandler<PersonAssignedToDelegationRemoved>,
        IElasticEventHandler<BodyAssignedToOrganisation>,
        IElasticEventHandler<BodyClearedFromOrganisation>
    {
        private readonly IContextFactory _contextFactory;
        private readonly IPersonHandlerCache _cache;

        public PersonMandate(
            ILogger<PersonMandate> logger,
            IContextFactory contextFactory,
            IPersonHandlerCache cache) : base(logger)
        {
            _contextFactory = contextFactory;
            _cache = cache;
        }


        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedPersonToBodySeat> message)
        {
            return new ElasticPerDocumentChange<PersonDocument>
            (
                message.Body.PersonId, async document =>
                {
                    await using var organisationRegistryContext = _contextFactory.Create();
                    var bodySeat = await organisationRegistryContext.BodySeatCache.FindRequiredAsync(message.Body.BodySeatId);

                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.Mandates == null)
                        document.Mandates = new List<PersonDocument.PersonMandate>();

                    document.Mandates.RemoveExistingListItems(x =>
                        x.BodyMandateId == message.Body.BodyMandateId &&
                        x.DelegationAssignmentId == null);

                    var organisationForBody = await _cache.GetOrganisationForBody(_contextFactory.Create(), message.Body.BodyId);

                    document.Mandates.Add(
                        new PersonDocument.PersonMandate(
                            message.Body.BodyMandateId,
                            null,
                            message.Body.BodyId,
                            bodySeat.Name,
                            organisationForBody.OrganisationId,
                            organisationForBody.OrganisationName,
                            message.Body.BodySeatId,
                            message.Body.BodySeatName,
                            message.Body.BodySeatNumber,
                            bodySeat.IsPaid,
                            Period.FromDates(message.Body.ValidFrom,
                                message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedPersonToBodySeat> message)
        {
            var changes = new Dictionary<Guid, Action<PersonDocument>>();

            // If previous exists and current is different, we need to delete and add
            if (message.Body.PreviousPersonId != message.Body.PersonId)
            {
                changes.Add(message.Body.PreviousPersonId, document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.Mandates == null)
                        document.Mandates = new List<PersonDocument.PersonMandate>();

                    document.Mandates.RemoveExistingListItems(x =>
                        x.BodyMandateId == message.Body.BodyMandateId &&
                        x.DelegationAssignmentId == null);
                });
            }

            changes.Add(message.Body.PersonId, async document =>
            {
                await using var organisationRegistryContext = _contextFactory.Create();
                var bodySeat = await organisationRegistryContext.BodySeatCache.FindRequiredAsync(message.Body.BodySeatId);

                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                if (document.Mandates == null)
                    document.Mandates = new List<PersonDocument.PersonMandate>();

                document.Mandates.RemoveExistingListItems(x =>
                    x.BodyMandateId == message.Body.BodyMandateId &&
                    x.DelegationAssignmentId == null);

                var organisationForBody = await _cache.GetOrganisationForBody(_contextFactory.Create(), message.Body.BodyId);

                document.Mandates.Add(
                    new PersonDocument.PersonMandate(
                        message.Body.BodyMandateId,
                        null,
                        message.Body.BodyId,
                        bodySeat.Name,
                        organisationForBody.OrganisationId,
                        organisationForBody.OrganisationName,
                        message.Body.BodySeatId,
                        message.Body.BodySeatName,
                        message.Body.BodySeatNumber,
                        bodySeat.IsPaid,
                        Period.FromDates(message.Body.ValidFrom,
                            message.Body.ValidTo)));
            });
            return new ElasticPerDocumentChange<PersonDocument>(changes);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyInfoChanged> message)
        {
            return new ElasticMassChange
            (
                async elastic => await elastic
                    .MassUpdatePersonAsync(
                        x => x.Mandates.Single().BodyId, message.Body.BodyId,
                        "mandates", "bodyId",
                        "bodyName", message.Body.Name,
                        message.Number,
                        message.Timestamp)
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
        {
            return new ElasticMassChange
            (
                async elastic =>
                {
                    await using var organisationRegistryContext = _contextFactory.Create();
                    var organisation = await organisationRegistryContext.OrganisationCache.SingleAsync(x => x.Id == message.Body.OrganisationId);

                    await elastic.TryAsync(() => elastic
                        .MassUpdatePersonAsync(
                            x => x.Mandates.Single().BodyId, message.Body.BodyId,
                            "mandates", "bodyId",
                            "bodyOrganisationId", message.Body.OrganisationId,
                            message.Number,
                            message.Timestamp));

                    await elastic.TryAsync(() => elastic
                        .MassUpdatePersonAsync(
                            x => x.Mandates.Single().BodyId, message.Body.BodyId,
                            "mandates", "bodyId",
                            "bodyOrganisationName", organisation.Id,
                            message.Number,
                            message.Timestamp));
                }
            );

        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatUpdated> message)
        {
            return new ElasticMassChange
            (
                async elastic =>
                {
                    await elastic.TryAsync(() => elastic
                        .MassUpdatePersonAsync(
                            x => x.Mandates.Single().BodySeatId, message.Body.BodySeatId,
                            "mandates", "bodySeatId",
                            "bodySeatName", message.Body.Name,
                            message.Number,
                            message.Timestamp));

                    await elastic.TryAsync(() => elastic
                        .MassUpdatePersonAsync(
                            x => x.Mandates.Single().BodySeatId, message.Body.BodySeatId,
                            "mandates", "bodySeatId",
                            "paidSeat", message.Body.PaidSeat,
                            message.Number,
                            message.Timestamp));
                }
            );

        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            return await MassUpdateMandateOrganisationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationNameUpdated> message)
        {
            return await MassUpdateMandateOrganisationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            return await MassUpdateMandateOrganisationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            return await MassUpdateMandateOrganisationName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling, message.Number, message.Timestamp);
        }

        private async Task<IElasticChange> MassUpdateMandateOrganisationName(Guid organisationId, string name, int messageNumber, DateTimeOffset timestamp)
        {
            return new ElasticMassChange
            (
                async elastic => await elastic
                    .MassUpdatePersonAsync(
                        x => x.Mandates.Single().BodyOrganisationId, organisationId,
                        "mandates", "bodyOrganisationId",
                        "bodyOrganisationName", name,
                        messageNumber,
                        timestamp)
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegation> message)
        {
            return new ElasticPerDocumentChange<PersonDocument>
            (
                message.Body.PersonId,
                async document =>
                {
                    await using var organisationRegistryContext = _contextFactory.Create();
                    var bodySeat = await organisationRegistryContext.BodySeatCache.FindRequiredAsync(message.Body.BodySeatId);
                    var body = await organisationRegistryContext.BodyCache.FindRequiredAsync(message.Body.BodyId);

                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.Mandates == null)
                        document.Mandates = new List<PersonDocument.PersonMandate>();

                    document.Mandates.RemoveExistingListItems(x =>
                        x.BodyMandateId == message.Body.BodyMandateId &&
                        x.DelegationAssignmentId == message.Body.DelegationAssignmentId);

                    var organisationForBody = await _cache.GetOrganisationForBody(_contextFactory.Create(), message.Body.BodyId);

                    document.Mandates.Add(
                        new PersonDocument.PersonMandate(
                            message.Body.BodyMandateId,
                            message.Body.DelegationAssignmentId,
                            message.Body.BodyId,
                            body.Name,
                            organisationForBody.OrganisationId,
                            organisationForBody.OrganisationName,
                            message.Body.BodySeatId,
                            bodySeat.Name,
                            bodySeat.Number,
                            bodySeat.IsPaid,
                            Period.FromDates(message.Body.ValidFrom,
                                message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationUpdated> message)
        {
            var changes = new Dictionary<Guid, Action<PersonDocument>>();

            // If previous exists and current is different, we need to delete and add
            if (message.Body.PreviousPersonId != message.Body.PersonId)
            {
                changes.Add(message.Body.PreviousPersonId, document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.Mandates == null)
                        document.Mandates = new List<PersonDocument.PersonMandate>();

                    document.Mandates.RemoveExistingListItems(x =>
                        x.BodyMandateId == message.Body.BodyMandateId &&
                        x.DelegationAssignmentId == message.Body.DelegationAssignmentId);
                });
            }

            changes.Add(message.Body.PersonId, async document =>
            {
                await using var organisationRegistryContext = _contextFactory.Create();
                var bodySeat = await organisationRegistryContext.BodySeatCache.FindRequiredAsync(message.Body.BodySeatId);
                var body = await organisationRegistryContext.BodyCache.FindRequiredAsync(message.Body.BodyId);

                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                if (document.Mandates == null)
                    document.Mandates = new List<PersonDocument.PersonMandate>();

                document.Mandates.RemoveExistingListItems(x =>
                    x.BodyMandateId == message.Body.BodyMandateId &&
                    x.DelegationAssignmentId == message.Body.DelegationAssignmentId);

                var organisationForBody = await _cache.GetOrganisationForBody(_contextFactory.Create(), message.Body.BodyId);

                document.Mandates.Add(
                    new PersonDocument.PersonMandate(
                        message.Body.BodyMandateId,
                        message.Body.DelegationAssignmentId,
                        message.Body.BodyId,
                        body.Name,
                        organisationForBody.OrganisationId,
                        organisationForBody.OrganisationName,
                        message.Body.BodySeatId,
                        bodySeat.Name,
                        bodySeat.Number,
                        bodySeat.IsPaid,
                        Period.FromDates(message.Body.ValidFrom,
                            message.Body.ValidTo)));
            });

            return new ElasticPerDocumentChange<PersonDocument>(changes);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationRemoved> message)
        {
            return new ElasticPerDocumentChange<PersonDocument>
            (
                message.Body.PreviousPersonId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.Mandates == null)
                        document.Mandates = new List<PersonDocument.PersonMandate>();

                    document.Mandates.RemoveExistingListItems(x =>
                        x.BodyMandateId == message.Body.BodyMandateId &&
                        x.DelegationAssignmentId == message.Body.DelegationAssignmentId);
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyAssignedToOrganisation> message)
        {
            return new ElasticMassChange(
                async elastic =>
                {
                    await using var organisationRegistryContext = _contextFactory.Create();
                    var organisation = await organisationRegistryContext.OrganisationCache.FindRequiredAsync(message.Body.OrganisationId);

                    await elastic.TryAsync(() => elastic
                        .MassUpdatePersonAsync(
                            x => x.Mandates.Single().BodyId, message.Body.BodyId,
                            "mandates", "BodyId",
                            "bodyOrganisationId", message.Body.OrganisationId,
                            message.Number,
                            message.Timestamp));

                    await elastic.TryAsync(() => elastic
                        .MassUpdatePersonAsync(
                            x => x.Mandates.Single().BodyId, message.Body.BodyId,
                            "mandates", "BodyId",
                            "bodyOrganisationName", organisation.Name,
                            message.Number,
                            message.Timestamp));

                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClearedFromOrganisation> message)
        {
            return new ElasticMassChange(
                async elastic =>
                {
                    await elastic.TryAsync(() => elastic
                        .MassUpdatePersonAsync(
                            x => x.Mandates.Single().BodyId, message.Body.BodyId,
                            "mandates", "bodyId",
                            "bodyOrganisationId", null,
                            message.Number,
                            message.Timestamp));

                    await elastic.TryAsync(() => elastic
                        .MassUpdatePersonAsync(
                            x => x.Mandates.Single().BodyId, message.Body.BodyId,
                            "mandates", "bodyId",
                            "bodyOrganisationName", null,
                            message.Number,
                            message.Timestamp));
                }
            );
        }
    }
}
