namespace OrganisationRegistry.SqlServer.Event
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.EventStore;

    public class EventListItem
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
        public string Data { get; set; }
        public string Ip { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string UserId { get; set; }
    }

    public class EventListConfiguration : EntityMappingConfiguration<EventListItem>
    {
        public override void Map(EntityTypeBuilder<EventListItem> b)
        {
            b.ToTable("Events", "OrganisationRegistry")
                .HasKey(p => new { p.Id, p.Version })
                .IsClustered();

            b.Property(p => p.Number)
                .IsRequired()
                .UseSqlServerIdentityColumn();

            b.Property(p => p.Id)
                .IsRequired();

            b.Property(p => p.Version)
                .IsRequired();

            b.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(SqlServerEventStore.NameLength);

            b.Property(p => p.Timestamp)
                .IsRequired();

            b.Property(p => p.Data)
                .IsRequired();

            b.Property(p => p.Ip)
                .HasMaxLength(SqlServerEventStore.IpLength);

            b.Property(p => p.LastName)
                .HasMaxLength(SqlServerEventStore.LastNameLength);

            b.Property(p => p.FirstName)
                .HasMaxLength(SqlServerEventStore.FirstNameLength);

            b.Property(p => p.UserId)
                .HasMaxLength(SqlServerEventStore.UserIdLength);

            b.HasIndex(x => x.Name);
            b.HasIndex(x => x.Number);
        }
    }
}
