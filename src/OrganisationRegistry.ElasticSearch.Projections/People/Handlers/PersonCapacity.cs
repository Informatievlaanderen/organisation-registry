namespace OrganisationRegistry.ElasticSearch.Projections.People.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Bodies;
    using Body;
    using Capacity.Events;
    using Client;
    using Common;
    using ElasticSearch.People;
    using Function.Events;
    using Infrastructure;
    using Infrastructure.Change;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Nest;
    using Organisation.Events;
    using SqlServer.ElasticSearchProjections;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer;

    public class PersonCapacity :
        Infrastructure.BaseProjection<PersonCapacity>,
        IElasticEventHandler<OrganisationCapacityAdded>,
        IElasticEventHandler<OrganisationCapacityUpdated>,
        IElasticEventHandler<CapacityUpdated>,
        IElasticEventHandler<FunctionUpdated>,
        IElasticEventHandler<OrganisationCreated>,
        IElasticEventHandler<OrganisationCreatedFromKbo>,
        IElasticEventHandler<OrganisationInfoUpdated>,
        IElasticEventHandler<OrganisationInfoUpdatedFromKbo>,
        IElasticEventHandler<OrganisationCouplingWithKboCancelled>,
        IElasticEventHandler<OrganisationCapacityBecameActive>,
        IElasticEventHandler<OrganisationCapacityBecameInactive>,
        IElasticEventHandler<OrganisationTerminated>
    {
        private readonly IContextFactory _contextFactory;

        private static readonly TimeSpan ScrollTimeout = TimeSpan.FromMinutes(5);

        public PersonCapacity(
            ILogger<PersonCapacity> logger,
            IContextFactory contextFactory) : base(logger)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<CapacityUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic => elastic.TryAsync(async () => await elastic
                    .MassUpdatePersonAsync(
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
                elastic => elastic.TryAsync(async () => await elastic
                    .MassUpdatePersonAsync(
                        x => x.Capacities.Single().FunctionId, message.Body.FunctionId,
                        "capacities", "functionId",
                        "functionName", message.Body.Name,
                        message.Number,
                        message.Timestamp))
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreated> message)
        {
            await AddCacheShowOnVlaamseOverheidSites(message.Body.OrganisationId, message.Body.ShowOnVlaamseOverheidSites);
            return new ElasticNoChange();
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreatedFromKbo> message)
        {
            await AddCacheShowOnVlaamseOverheidSites(message.Body.OrganisationId, false);
            return new ElasticNoChange();
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            return new ElasticMassChange
            (
                async elastic =>
                {
                    await MassUpdateOrganisationName(elastic, message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);

                    // If ShowOnVlaamseOverheid changed, update ShowOnVlaamseOverheid for all people needed
                    if (message.Body.ShowOnVlaamseOverheidSites == message.Body.PreviouslyShownInVlaanderenBe)
                        return;

                    await UpdateCacheShowOnVlaamseOverheidSites(message);

                    await ElasticWriter.UpdateByScroll<PersonDocument>(elastic,
                        query => query.Nested(
                            nested => nested.Path(
                                path => path.Capacities).Query(
                                innerQuery => innerQuery.Bool(
                                    b => b.Must(
                                        must => must.Match(
                                            match => match
                                                .Field("capacities.organisationId")
                                                .Query(message.Body.OrganisationId.ToString())))))),
                        async document => document.ShowOnVlaamseOverheidSites =
                            await ShouldPersonBeShownOnVlaamseOverheidSites(document));
                });
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            return new ElasticMassChange(elastic =>
                MassUpdateOrganisationName(
                    elastic, message.Body.OrganisationId, message.Body.Name,
                    message.Number, message.Timestamp));
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            return new ElasticMassChange(elastic =>
                MassUpdateOrganisationName(
                    elastic, message.Body.OrganisationId, message.Body.NameBeforeKboCoupling,
                    message.Number, message.Timestamp));
        }

        private async Task MassUpdateOrganisationName(Elastic elastic, Guid organisationId, string name, int messageNumber, DateTimeOffset timestamp)
        {
            await elastic.TryAsync(async () => await elastic
                .MassUpdatePersonAsync(
                    x => x.Capacities.Single().OrganisationId, organisationId,
                    "capacities", "organisationId",
                    "organisationName", name,
                    messageNumber,
                    timestamp));
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityAdded> message)
        {
            // Person is optional
            if (!message.Body.PersonId.HasValue)
                return new ElasticNoChange();

            return new ElasticPerDocumentChange<PersonDocument>(
                message.Body.PersonId.Value, async document =>
                {
                    await using var organisationRegistryContext = _contextFactory.Create();
                    var organisation = await organisationRegistryContext.OrganisationCache.SingleAsync(x => x.Id == message.Body.OrganisationId);
                    var contactTypeNames = await organisationRegistryContext.ContactTypeList
                        .Select(x => new {x.Id, x.Name})
                        .ToDictionaryAsync(x => x.Id, x => x.Name);

                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.Capacities == null)
                        document.Capacities = new List<PersonDocument.PersonCapacity>();

                    document.Capacities.RemoveExistingListItems(x =>
                        x.PersonCapacityId == message.Body.OrganisationCapacityId);

                    document.Capacities.Add(
                        new PersonDocument.PersonCapacity(
                            message.Body.OrganisationCapacityId,
                            message.Body.CapacityId,
                            message.Body.CapacityName,
                            message.Body.OrganisationId,
                            organisation.Name,
                            message.Body.FunctionId,
                            message.Body.FunctionName,
                            message.Body.Contacts.Select(x => new Contact(x.Key, contactTypeNames[x.Key], x.Value))
                                .ToList(),
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));

                    document.ShowOnVlaamseOverheidSites =
                        await ShouldPersonBeShownOnVlaamseOverheidSites(document);
                });
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityUpdated> message)
        {
            // If previous and current person are null, we dont care
            if (!message.Body.PreviousPersonId.HasValue && !message.Body.PersonId.HasValue)
                return new ElasticNoChange();

            var changes = new Dictionary<Guid, Action<PersonDocument>>();
            // If previous exists and current is different, we need to delete and add
            if (message.Body.PreviousPersonId.HasValue &&
                message.Body.PreviousPersonId != message.Body.PersonId)
            {
                changes.Add(
                    message.Body.PreviousPersonId.Value, async document =>
                    {
                        document.ChangeId = message.Number;
                        document.ChangeTime = message.Timestamp;

                        if (document.Capacities == null)
                            document.Capacities = new List<PersonDocument.PersonCapacity>();

                        document.Capacities.RemoveExistingListItems(x =>
                            x.PersonCapacityId == message.Body.OrganisationCapacityId);

                        document.ShowOnVlaamseOverheidSites = await ShouldPersonBeShownOnVlaamseOverheidSites(document);
                    });
            }

            if (message.Body.PersonId.HasValue)
            {
                changes.Add(
                    message.Body.PersonId.Value, async document =>
                    {
                        await using var organisationRegistryContext = _contextFactory.Create();
                        var organisation =
                            await organisationRegistryContext.OrganisationCache.FindAsync(message.Body.OrganisationId);
                        var contactTypeNames = await organisationRegistryContext.ContactTypeList
                            .Select(x => new {x.Id, x.Name})
                            .ToDictionaryAsync(x => x.Id, x => x.Name);

                        document.ChangeId = message.Number;
                        document.ChangeTime = message.Timestamp;

                        if (document.Capacities == null)
                            document.Capacities = new List<PersonDocument.PersonCapacity>();

                        document.Capacities.RemoveExistingListItems(x =>
                            x.PersonCapacityId == message.Body.OrganisationCapacityId);

                        document.Capacities.Add(
                            new PersonDocument.PersonCapacity(
                                message.Body.OrganisationCapacityId,
                                message.Body.CapacityId,
                                message.Body.CapacityName,
                                message.Body.OrganisationId,
                                organisation.Name,
                                message.Body.FunctionId,
                                message.Body.FunctionName,
                                message.Body.Contacts.Select(x => new Contact(x.Key, contactTypeNames[x.Key], x.Value))
                                    .ToList(),
                                new Period(message.Body.ValidFrom, message.Body.ValidTo)));

                        document.ShowOnVlaamseOverheidSites = await ShouldPersonBeShownOnVlaamseOverheidSites(document);
                    }
                );
            }

            return new ElasticPerDocumentChange<PersonDocument>(changes);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityBecameActive> message)
        {
            await UpdateIsActivePerOrganisationCapacity(message.Body.OrganisationCapacityId, true);
            return await UpdateShouldBeShownOnVlaamseOverheidSites(message.Body.PersonId);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityBecameInactive> message)
        {
            await UpdateIsActivePerOrganisationCapacity(message.Body.OrganisationCapacityId, false);
            return await UpdateShouldBeShownOnVlaamseOverheidSites(message.Body.PersonId);
        }

        private async Task<IElasticChange> UpdateShouldBeShownOnVlaamseOverheidSites(Guid? personId)
        {
            if (!personId.HasValue)
                return new ElasticNoChange();

            return new ElasticPerDocumentChange<PersonDocument>
            (
                personId.Value,
                async document => document.ShowOnVlaamseOverheidSites = await ShouldPersonBeShownOnVlaamseOverheidSites(document)
            );
        }

        private async Task<bool> ShouldPersonBeShownOnVlaamseOverheidSites(PersonDocument personDocument)
        {
            await using var context = _contextFactory.Create();

            var organisationIds =
                personDocument.Capacities
                    .Select(capacity => capacity.OrganisationId)
                    .ToList();

            var organisationsInPersonCapacitiesShownOnVlaamseOverheidSites =
                context.ShowOnVlaamseOverheidSitesPerOrganisationList
                    .AsNoTracking()
                    .Where(organisation => organisationIds.Contains(organisation.Id))
                    .Where(organisation => organisation.ShowOnVlaamseOverheidSites)
                    .Select(organisation => organisation.Id)
                    .ToList();

            var capacitiesWithOrganisationShownOnVlaamseOverheidSites =
                personDocument.Capacities
                    .Where(capacity => organisationsInPersonCapacitiesShownOnVlaamseOverheidSites.Contains(capacity.OrganisationId))
                    .Select(capacity => capacity.PersonCapacityId)
                    .ToList();

            return context.IsActivePerOrganisationCapacityList
                .AsNoTracking()
                .Where(capacity => capacity.IsActive)
                .Any(capacity => capacitiesWithOrganisationShownOnVlaamseOverheidSites.Contains(capacity.OrganisationCapacityId));
        }

        private async Task AddCacheShowOnVlaamseOverheidSites(Guid organisationId, bool showOnVlaamseOverheidSites)
        {
            await using var context = _contextFactory.Create();
            if (context.ShowOnVlaamseOverheidSitesPerOrganisationList.Any(x => x.Id == organisationId))
                return;

            context
                .ShowOnVlaamseOverheidSitesPerOrganisationList
                .Add(new ShowOnVlaamseOverheidSitesPerOrganisation
                {
                    Id = organisationId,
                    ShowOnVlaamseOverheidSites = showOnVlaamseOverheidSites
                });

            await context.SaveChangesAsync();
        }

        private async Task UpdateCacheShowOnVlaamseOverheidSites(IEnvelope<OrganisationInfoUpdated> message)
        {
            await using var context = _contextFactory.Create();
            var showOnVlaamseOverheidSitesPerOrganisation =
                await context
                    .ShowOnVlaamseOverheidSitesPerOrganisationList
                    .FindAsync(message.Body.OrganisationId);

            showOnVlaamseOverheidSitesPerOrganisation.ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites;

            await context.SaveChangesAsync();
        }

        private async Task UpdateIsActivePerOrganisationCapacity(Guid organisationCapacityId, bool isActive)
        {
            await using var context = _contextFactory.Create();
            var isActivePerOrganisationCapacity =
                await context
                    .IsActivePerOrganisationCapacityList
                    .FindAsync(organisationCapacityId);

            if (isActivePerOrganisationCapacity == null)
            {
                context
                    .IsActivePerOrganisationCapacityList
                    .Add(new IsActivePerOrganisationCapacity
                    {
                        OrganisationCapacityId = organisationCapacityId,
                        IsActive = isActive
                    });
            }
            else
            {
                isActivePerOrganisationCapacity.IsActive = isActive;
            }

            await context.SaveChangesAsync();
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            return new ElasticMassChange
            (
                async elastic =>
                {
                    foreach (var (key, value) in message.Body.FieldsToTerminate.Capacities)
                    {
                        await elastic.TryAsync(() => elastic
                            .MassUpdatePersonAsync(
                                queryFieldSelector: x => x.Capacities.Single().CapacityId, queryFieldValue: key,
                                listPropertyName: "capacities", idPropertyName: "capacityId",
                                namePropertyName: "validity.end", newName: value,
                                changeId: message.Number,
                                changeTime: message.Timestamp));
                    }
                });
        }
    }
}
