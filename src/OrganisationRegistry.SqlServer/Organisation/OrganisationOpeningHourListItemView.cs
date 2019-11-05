namespace OrganisationRegistry.SqlServer.Organisation
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Data.Common;
    using System.Linq;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    public class OrganisationOpeningHourListItem
    {
        public Guid OrganisationOpeningHourId { get; set; }

        public Guid OrganisationId { get; set; }

        public TimeSpan Opens { get; set; }

        public TimeSpan Closes { get; set; }

        public DayOfWeek? DayOfWeek { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public OrganisationOpeningHourListItem()
        {
        }

        public OrganisationOpeningHourListItem(
            Guid organisationOpeningHourId,
            Guid organisationId,
            TimeSpan opens,
            TimeSpan closes,
            DayOfWeek dayOfWeek,
            DateTime? validFrom,
            DateTime? validTo)
        {
            OrganisationOpeningHourId = organisationOpeningHourId;
            OrganisationId = organisationId;
            Opens = opens;
            Closes = closes;
            DayOfWeek = dayOfWeek;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }

    public class OrganisationOpeningHourListConfiguration : EntityMappingConfiguration<OrganisationOpeningHourListItem>
    {
        public override void Map(EntityTypeBuilder<OrganisationOpeningHourListItem> b)
        {
            b.ToTable(nameof(OrganisationOpeningHourListItemView.ProjectionTables.OrganisationOpeningHourList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationOpeningHourId)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.Opens);
            b.Property(p => p.Closes);
            b.Property(p => p.DayOfWeek);

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);
        }
    }

    public class OrganisationOpeningHourListItemView :
        Projection<OrganisationOpeningHourListItemView>,
        IEventHandler<OrganisationOpeningHourAdded>,
        IEventHandler<OrganisationOpeningHourUpdated>
    {
        private readonly IEventStore _eventStore;

        public OrganisationOpeningHourListItemView(
            ILogger<OrganisationOpeningHourListItemView> logger,
            IEventStore eventStore)
            : base(logger)
        {
            _eventStore = eventStore;
        }

        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationOpeningHourList
        }

        public override void Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }

        public void Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationOpeningHourAdded> message)
        {
            var openingHourListItem = new OrganisationOpeningHourListItem
            {
                OrganisationOpeningHourId = message.Body.OrganisationOpeningHourId,
                OrganisationId = message.Body.OrganisationId,
                Opens = message.Body.Opens,
                Closes = message.Body.Closes,
                DayOfWeek = message.Body.DayOfWeek,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.OrganisationOpeningHourList.Add(openingHourListItem);
                context.SaveChanges();
            }
        }

        public void Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationOpeningHourUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var label = context.OrganisationOpeningHourList.SingleOrDefault(item => item.OrganisationOpeningHourId == message.Body.OrganisationOpeningHourId);

                label.OrganisationOpeningHourId = message.Body.OrganisationOpeningHourId;
                label.OrganisationId = message.Body.OrganisationId;
                label.Opens = message.Body.Opens;
                label.Closes = message.Body.Closes;
                label.DayOfWeek = message.Body.DayOfWeek;
                label.ValidFrom = message.Body.ValidFrom;
                label.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }
    }
}
