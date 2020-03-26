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
    using Capacity;
    using FunctionType;
    using Location;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Person;
    using OrganisationRegistry.Function.Events;
    using OrganisationRegistry.Capacity.Events;
    using OrganisationRegistry.Location.Events;
    using OrganisationRegistry.Person.Events;

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

        public string ContactsJson { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
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
        IEventHandler<LocationUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationCapacityList
        }

        private readonly IEventStore _eventStore;

        public OrganisationCapacityListView(
            ILogger<OrganisationCapacityListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<CapacityUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var organisationCapacities = context.OrganisationCapacityList.Where(x => x.CapacityId == message.Body.CapacityId);
                if (!organisationCapacities.Any())
                    return;

                foreach (var organisationCapacity in organisationCapacities)
                    organisationCapacity.CapacityName = message.Body.Name;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var organisationCapacities = context.OrganisationCapacityList.Where(x => x.FunctionId == message.Body.FunctionId);
                if (!organisationCapacities.Any())
                    return;

                foreach (var organisationCapacity in organisationCapacities)
                    organisationCapacity.FunctionName = message.Body.Name;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var organisationCapacities = context.OrganisationCapacityList.Where(x => x.PersonId == message.Body.PersonId);
                if (!organisationCapacities.Any())
                    return;

                foreach (var organisationCapacity in organisationCapacities)
                    organisationCapacity.PersonName = $"{message.Body.FirstName} {message.Body.Name}";

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LocationUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var organisationCapacities = context.OrganisationCapacityList.Where(x => x.LocationId == message.Body.LocationId);
                if (!organisationCapacities.Any())
                    return;

                foreach (var organisationCapacity in organisationCapacities)
                    organisationCapacity.LocationName = message.Body.FormattedAddress;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityAdded> message)
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

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.OrganisationCapacityList.Add(organisationCapacityListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var key = context.OrganisationCapacityList.SingleOrDefault(item => item.OrganisationCapacityId == message.Body.OrganisationCapacityId);

                key.OrganisationCapacityId = message.Body.OrganisationCapacityId;
                key.OrganisationId = message.Body.OrganisationId;
                key.CapacityId = message.Body.CapacityId;
                key.PersonId = message.Body.PersonId;
                key.FunctionId = message.Body.FunctionId;
                key.LocationId = message.Body.LocationId;
                key.CapacityName = message.Body.CapacityName;
                key.PersonName = message.Body.PersonId.HasValue ? message.Body.PersonFullName : string.Empty;
                key.FunctionName = message.Body.FunctionId.HasValue ? message.Body.FunctionName : string.Empty;
                key.LocationName = message.Body.LocationId.HasValue ? message.Body.LocationName : string.Empty;
                key.ContactsJson = JsonConvert.SerializeObject(message.Body.Contacts ?? new Dictionary<Guid, string>());
                key.ValidFrom = message.Body.ValidFrom;
                key.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
