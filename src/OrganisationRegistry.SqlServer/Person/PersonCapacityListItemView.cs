namespace OrganisationRegistry.SqlServer.Person
{
    using System;
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    using System.Linq;
    using System.Threading.Tasks;
    using Capacity;
    using FunctionType;
    using Microsoft.Extensions.Logging;
    using Organisation;
    using OrganisationRegistry.Capacity.Events;
    using Function.Events;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.AppSpecific;

    public class PersonCapacityListItem
    {
        public Guid OrganisationCapacityId { get; set; }

        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }

        public Guid CapacityId { get; set; }
        public string CapacityName { get; set; }

        public Guid? PersonId { get; set; }

        public Guid? FunctionId { get; set; }
        public string? FunctionName { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class PersonCapacityListConfiguration : EntityMappingConfiguration<PersonCapacityListItem>
    {
        public override void Map(EntityTypeBuilder<PersonCapacityListItem> b)
        {
            b.ToTable(nameof(PersonCapacityListView.ProjectionTables.PersonCapacityList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.OrganisationCapacityId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();
            b.Property(p => p.OrganisationName).HasMaxLength(OrganisationListConfiguration.NameLength).IsRequired();

            b.Property(p => p.CapacityId).IsRequired();
            b.Property(p => p.CapacityName).HasMaxLength(CapacityListConfiguration.NameLength).IsRequired();

            b.Property(p => p.FunctionId);
            b.Property(p => p.FunctionName).HasMaxLength(FunctionTypeListConfiguration.NameLength);

            b.Property(p => p.PersonId).IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.OrganisationName).IsClustered();
            b.HasIndex(x => x.CapacityName);
            b.HasIndex(x => x.FunctionName);
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class PersonCapacityListView :
        Projection<PersonCapacityListView>,
        IEventHandler<OrganisationCapacityAdded>,
        IEventHandler<OrganisationCapacityUpdated>,
        IEventHandler<CapacityUpdated>,
        IEventHandler<FunctionUpdated>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
        IEventHandler<OrganisationTerminated>,
        IEventHandler<OrganisationTerminatedV2>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            PersonCapacityList
        }

        private readonly IEventStore _eventStore;
        private readonly IMemoryCaches _memoryCaches;

        public PersonCapacityListView(
            ILogger<PersonCapacityListView> logger,
            IEventStore eventStore,
            IMemoryCaches memoryCaches,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
            _memoryCaches = memoryCaches;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            await UpdateOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.Name);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            await UpdateOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.Name);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            await UpdateOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.NameBeforeKboCoupling);
        }

        private static async Task UpdateOrganisationName(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IContextFactory contextFactory,
            Guid organisationId,
            string organisationName)
        {
            await using var context = contextFactory.CreateTransactional(dbConnection, dbTransaction);
            var personCapacities = context.PersonCapacityList.Where(x => x.OrganisationId == organisationId);
            if (!personCapacities.Any())
                return;

            foreach (var personCapacity in personCapacities)
                personCapacity.OrganisationName = organisationName;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<CapacityUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var personCapacities = context.PersonCapacityList.Where(x => x.CapacityId == message.Body.CapacityId);
            if (!personCapacities.Any())
                return;

            foreach (var personCapacity in personCapacities)
                personCapacity.CapacityName = message.Body.Name;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var personCapacities = context.PersonCapacityList.Where(x => x.FunctionId == message.Body.FunctionId);
            if (!personCapacities.Any())
                return;

            foreach (var personCapacity in personCapacities)
                personCapacity.FunctionName = message.Body.Name;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityAdded> message)
        {
            if (!message.Body.PersonId.HasValue)
                return;

            var personCapacityListItem = new PersonCapacityListItem
            {
                OrganisationCapacityId = message.Body.OrganisationCapacityId,
                OrganisationId = message.Body.OrganisationId,
                OrganisationName = _memoryCaches.OrganisationNames[message.Body.OrganisationId],
                CapacityId = message.Body.CapacityId,
                CapacityName = message.Body.CapacityName,
                PersonId = message.Body.PersonId,
                FunctionId = message.Body.FunctionId,
                FunctionName = message.Body.FunctionId.HasValue ? message.Body.FunctionName : string.Empty,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            await context.PersonCapacityList.AddAsync(personCapacityListItem);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityUpdated> message)
        {
            if (message.Body.PersonId.HasValue)
            {
                await AddOrUpdatePersonCapacity(dbConnection, dbTransaction, message);
            }
            else
            {
                await RemovePersonCapacity(dbConnection, dbTransaction, message);
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }

        private async Task AddOrUpdatePersonCapacity(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var personCapacityListItem = context.PersonCapacityList.SingleOrDefault(item => item.OrganisationCapacityId == message.Body.OrganisationCapacityId);

            if (personCapacityListItem == null)
            {
                personCapacityListItem = new PersonCapacityListItem();
                context.PersonCapacityList.Add(personCapacityListItem);
            }

            personCapacityListItem.OrganisationCapacityId = message.Body.OrganisationCapacityId;
            personCapacityListItem.OrganisationId = message.Body.OrganisationId;
            personCapacityListItem.OrganisationName = _memoryCaches.OrganisationNames[message.Body.OrganisationId];
            personCapacityListItem.CapacityId = message.Body.CapacityId;
            personCapacityListItem.CapacityName = message.Body.CapacityName;
            personCapacityListItem.PersonId = message.Body.PersonId;
            personCapacityListItem.FunctionId = message.Body.FunctionId;
            personCapacityListItem.FunctionName = message.Body.FunctionId.HasValue ? message.Body.FunctionName : string.Empty;
            personCapacityListItem.ValidFrom = message.Body.ValidFrom;
            personCapacityListItem.ValidTo = message.Body.ValidTo;

            await context.SaveChangesAsync();
        }

        private async Task RemovePersonCapacity(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var personCapacityListItem = context.PersonCapacityList.SingleOrDefault(item => item.OrganisationCapacityId == message.Body.OrganisationCapacityId);

            if (personCapacityListItem == null)
                return;

            context.PersonCapacityList.Remove(personCapacityListItem);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var capacityListItems = context.PersonCapacityList.Where(item =>
                message.Body.FieldsToTerminate.Capacities.Keys.Contains(item.OrganisationCapacityId));

            foreach (var capacityListItem in capacityListItems)
                capacityListItem.ValidTo = message.Body.FieldsToTerminate.Capacities[capacityListItem.OrganisationCapacityId];

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var capacityListItems = context.PersonCapacityList.Where(item =>
                message.Body.FieldsToTerminate.Capacities.Keys.Contains(item.OrganisationCapacityId));

            foreach (var capacityListItem in capacityListItems)
                capacityListItem.ValidTo = message.Body.FieldsToTerminate.Capacities[capacityListItem.OrganisationCapacityId];

            await context.SaveChangesAsync();
        }
    }
}
