namespace OrganisationRegistry.SqlServer.Location
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Location.Events;

    public class LocationListItem
    {
        public Guid Id { get; set; }

        public string? CrabLocationId { get; set; }
        public string? FormattedAddress { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public bool HasCrabLocation { get; set; }
    }

    public class LocationListConfiguration : EntityMappingConfiguration<LocationListItem>
    {
        public const int CityLength = 100;
        public const int ZipCodeLength = 50;
        public const int StreetLength = 200;
        public const int CountryLength = 100;
        public const int FormattedAddressLength = CityLength + ZipCodeLength + StreetLength + CountryLength + 10; // 10 for spaces and commas

        public override void Map(EntityTypeBuilder<LocationListItem> b)
        {
            b.ToTable(nameof(LocationListView.ProjectionTables.LocationList), WellknownSchemas.OrganisationRegistrySchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.CrabLocationId);

            b.Property(p => p.FormattedAddress)
                .HasMaxLength(FormattedAddressLength);

            b.Property(p => p.Street)
                .HasMaxLength(StreetLength)
                .IsRequired();

            b.Property(p => p.ZipCode)
                .HasMaxLength(ZipCodeLength)
                .IsRequired();

            b.Property(p => p.City)
                .HasMaxLength(CityLength)
                .IsRequired();

            b.Property(p => p.Country)
                .HasMaxLength(CountryLength)
                .IsRequired();

            b.Property(p => p.HasCrabLocation);

            b.HasIndex(x => x.FormattedAddress).IsClustered();
            b.HasIndex(x => x.Street);
            b.HasIndex(x => x.ZipCode);
            b.HasIndex(x => x.City);
            b.HasIndex(x => x.Country);
            b.HasIndex(x => x.HasCrabLocation);
        }
    }

    public class LocationListView :
        Projection<LocationListView>,
        IEventHandler<LocationCreated>,
        IEventHandler<LocationUpdated>
    {
        private readonly IEventStore _eventStore;

        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            LocationList
        }

        public LocationListView(
            ILogger<LocationListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LocationCreated> message)
        {
            var location = new LocationListItem
            {
                Id = message.Body.LocationId,
                CrabLocationId = message.Body.CrabLocationId,
                FormattedAddress = message.Body.FormattedAddress,
                Street = message.Body.Street,
                ZipCode = message.Body.ZipCode,
                City = message.Body.City,
                Country = message.Body.Country,
                HasCrabLocation = !string.IsNullOrWhiteSpace(message.Body.CrabLocationId)
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.LocationList.AddAsync(location);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LocationUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var location = context.LocationList.SingleOrDefault(x => x.Id == message.Body.LocationId);
                if (location == null)
                    return; // TODO: Error?

                location.CrabLocationId = message.Body.CrabLocationId;
                location.FormattedAddress = message.Body.FormattedAddress;
                location.Street = message.Body.Street;
                location.ZipCode = message.Body.ZipCode;
                location.City = message.Body.City;
                location.Country = message.Body.Country;
                location.HasCrabLocation = !string.IsNullOrWhiteSpace(message.Body.CrabLocationId);
                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
