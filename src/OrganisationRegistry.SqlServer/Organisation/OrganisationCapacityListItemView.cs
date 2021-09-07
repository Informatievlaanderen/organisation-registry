namespace OrganisationRegistry.SqlServer.Organisation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    using System.Linq;
    using System.Threading.Tasks;
    using Capacity;
    using Day.Events;
    using FunctionType;
    using Location;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Person;
    using Function.Events;
    using OrganisationRegistry.Capacity.Events;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Location.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Person.Events;
    using RebuildProjection = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

    public class OrganisationCapacityListItem
    {
        public Guid OrganisationCapacityId { get; set; }
        public Guid OrganisationId { get; set; }

        public Guid CapacityId { get; set; }
        public string CapacityName { get; set; }

        public Guid? PersonId { get; set; }
        public string? PersonName { get; set; }

        public Guid? FunctionId { get; set; }
        public string? FunctionName { get; set; }

        public Guid? LocationId { get; set; }
        public string? LocationName { get; set; }

        public string? ContactsJson { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool? IsActive { get; set; }
    }

    public class OrganisationCapacityListConfiguration : EntityMappingConfiguration<OrganisationCapacityListItem>
    {
        public override void Map(EntityTypeBuilder<OrganisationCapacityListItem> b)
        {
            b.ToTable(nameof(OrganisationCapacityListView.ProjectionTables.OrganisationCapacityList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationCapacityId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.CapacityId).IsRequired();
            b.Property(p => p.CapacityName).HasMaxLength(CapacityListConfiguration.NameLength).IsRequired();

            b.Property(p => p.PersonId);
            b.Property(p => p.PersonName).HasMaxLength(PersonListConfiguration.FullNameLength);

            b.Property(p => p.FunctionId);
            b.Property(p => p.FunctionName).HasMaxLength(FunctionTypeListConfiguration.NameLength);

            b.Property(p => p.LocationId);
            b.Property(p => p.LocationName).HasMaxLength(LocationListConfiguration.FormattedAddressLength);

            b.Property(p => p.ContactsJson);

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);
            b.Property(p => p.IsActive);

            b.HasIndex(x => x.CapacityName).IsClustered();
            b.HasIndex(x => x.PersonName);
            b.HasIndex(x => x.FunctionName);
            b.HasIndex(x => x.LocationName);
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class OrganisationCapacityListView :
        Projection<OrganisationCapacityListView>,
        IEventHandler<OrganisationCapacityAdded>,
        IEventHandler<OrganisationCapacityUpdated>,
        IEventHandler<CapacityUpdated>,
        IEventHandler<FunctionUpdated>,
        IEventHandler<PersonUpdated>,
        IEventHandler<LocationUpdated>,
        IEventHandler<OrganisationTerminated>,
        IEventHandler<OrganisationTerminatedV2>,
        IEventHandler<OrganisationCapacityBecameActive>,
        IEventHandler<OrganisationCapacityBecameInactive>,
        IReactionHandler<DayHasPassed>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationCapacityList
        }

        private readonly IEventStore _eventStore;

        public OrganisationCapacityListView(
            ILogger<OrganisationCapacityListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory = null) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<CapacityUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var organisationCapacities = context.OrganisationCapacityList.Where(x => x.CapacityId == message.Body.CapacityId);
            if (!organisationCapacities.Any())
                return;

            foreach (var organisationCapacity in organisationCapacities)
                organisationCapacity.CapacityName = message.Body.Name;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var organisationCapacities = context.OrganisationCapacityList.Where(x => x.FunctionId == message.Body.FunctionId);
            if (!organisationCapacities.Any())
                return;

            foreach (var organisationCapacity in organisationCapacities)
                organisationCapacity.FunctionName = message.Body.Name;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var organisationCapacities = context.OrganisationCapacityList.Where(x => x.PersonId == message.Body.PersonId);
            if (!organisationCapacities.Any())
                return;

            foreach (var organisationCapacity in organisationCapacities)
                organisationCapacity.PersonName = $"{message.Body.FirstName} {message.Body.Name}";

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LocationUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var organisationCapacities = context.OrganisationCapacityList.Where(x => x.LocationId == message.Body.LocationId);
            if (!organisationCapacities.Any())
                return;

            foreach (var organisationCapacity in organisationCapacities)
                organisationCapacity.LocationName = message.Body.FormattedAddress;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityAdded> message)
        {
            var organisationCapacityListItem = new OrganisationCapacityListItem
            {
                OrganisationCapacityId = message.Body.OrganisationCapacityId,
                OrganisationId = message.Body.OrganisationId,
                CapacityId = message.Body.CapacityId,
                PersonId = message.Body.PersonId,
                FunctionId = message.Body.FunctionId,
                LocationId = message.Body.LocationId,
                CapacityName = message.Body.CapacityName,
                PersonName = message.Body.PersonId.HasValue ? message.Body.PersonFullName : string.Empty,
                FunctionName = message.Body.FunctionId.HasValue ? message.Body.FunctionName : string.Empty,
                LocationName = message.Body.LocationId.HasValue ? message.Body.LocationName : string.Empty,
                ContactsJson = JsonConvert.SerializeObject(message.Body.Contacts ?? new Dictionary<Guid, string>()),
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            await context.OrganisationCapacityList.AddAsync(organisationCapacityListItem);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var capacity = await context.OrganisationCapacityList.SingleAsync(item => item.OrganisationCapacityId == message.Body.OrganisationCapacityId);

            capacity.OrganisationCapacityId = message.Body.OrganisationCapacityId;
            capacity.OrganisationId = message.Body.OrganisationId;
            capacity.CapacityId = message.Body.CapacityId;
            capacity.PersonId = message.Body.PersonId;
            capacity.FunctionId = message.Body.FunctionId;
            capacity.LocationId = message.Body.LocationId;
            capacity.CapacityName = message.Body.CapacityName;
            capacity.PersonName = message.Body.PersonId.HasValue ? message.Body.PersonFullName : string.Empty;
            capacity.FunctionName = message.Body.FunctionId.HasValue ? message.Body.FunctionName : string.Empty;
            capacity.LocationName = message.Body.LocationId.HasValue ? message.Body.LocationName : string.Empty;
            capacity.ContactsJson = JsonConvert.SerializeObject(message.Body.Contacts ?? new Dictionary<Guid, string>());
            capacity.ValidFrom = message.Body.ValidFrom;
            capacity.ValidTo = message.Body.ValidTo;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var capacities = context.OrganisationCapacityList.Where(item =>
                message.Body.FieldsToTerminate.Capacities.Keys.Contains(item.OrganisationCapacityId));

            foreach (var capacity in capacities)
                capacity.ValidTo = message.Body.FieldsToTerminate.Capacities[capacity.OrganisationCapacityId];

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var capacities = context.OrganisationCapacityList.Where(item =>
                message.Body.FieldsToTerminate.Capacities.Keys.Contains(item.OrganisationCapacityId));

            foreach (var capacity in capacities)
                capacity.ValidTo = message.Body.FieldsToTerminate.Capacities[capacity.OrganisationCapacityId];

            await context.SaveChangesAsync();        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<OrganisationCapacityBecameActive> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var capacity = await context.OrganisationCapacityList.SingleAsync(item => item.OrganisationCapacityId == message.Body.OrganisationCapacityId);
            capacity.IsActive = true;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityBecameInactive> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var capacity = await context.OrganisationCapacityList.SingleAsync(item => item.OrganisationCapacityId == message.Body.OrganisationCapacityId);
            capacity.IsActive = false;

            await context.SaveChangesAsync();
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }

        public async Task<List<ICommand>> Handle(IEnvelope<DayHasPassed> message)
        {
            await using var context = ContextFactory.Create();
            var undetermined =
                context.OrganisationCapacityList
                    .Where(item =>
                        !item.IsActive.HasValue)
                    .Select(item => item.OrganisationId)
                    .Distinct();

            var shouldBeActive =
                context.OrganisationCapacityList
                    .Where(item => item.IsActive.HasValue &&
                                   !item.IsActive.Value &&
                                   ((item.ValidFrom == null || item.ValidFrom <= message.Body.Date) &&
                                    (item.ValidTo == null || item.ValidTo >= message.Body.Date)))
                    .Select(item => item.OrganisationId)
                    .Distinct();

            var shouldBeInactive =
                context.OrganisationCapacityList
                    .Where(item => item.IsActive.HasValue &&
                                   item.IsActive.Value &&
                                   ((item.ValidFrom != null && item.ValidFrom > message.Body.Date) &&
                                    (item.ValidTo != null && item.ValidTo < message.Body.Date)))
                    .Select(item => item.OrganisationId)
                    .Distinct();

            return await undetermined
                .Union(shouldBeActive)
                .Union(shouldBeInactive)
                .Distinct()
                .Select(id => new UpdateRelationshipValidities(new OrganisationId(id), message.Body.NextDate))
                .Cast<ICommand>()
                .ToListAsync();
        }
    }
}
