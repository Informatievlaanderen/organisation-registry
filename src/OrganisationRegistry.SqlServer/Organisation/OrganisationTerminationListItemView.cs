namespace OrganisationRegistry.SqlServer.Organisation
{
    using System;
    using System.Data.Common;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    public class OrganisationTerminationListItem
    {
        public Guid Id { get; set; }

        public string OvoNumber { get; set; }

        public string? KboNumber { get; set; }

        public string Name { get; set; }

        public TerminationStatus Status { get; set; }
        public DateTime Date { get; set; }
        public string Code { get; set; }
        public string Reason { get; set; }
    }

    public class OrganisationTerminationListConfiguration : EntityMappingConfiguration<OrganisationTerminationListItem>
    {
        public override void Map(EntityTypeBuilder<OrganisationTerminationListItem> b)
        {
            b.ToTable(nameof(OrganisationTerminationListItemView.ProjectionTables.OrganisationTerminationList),
                    "OrganisationRegistry")
                .HasKey(p => new {p.Id, p.KboNumber})
                .IsClustered(false);

            b.Property(p => p.OvoNumber).HasMaxLength(OrganisationListConfiguration.OvoNumberLength).IsRequired();

            b.Property(p => p.Name).HasMaxLength(OrganisationListConfiguration.NameLength).IsRequired();
            b.HasIndex(x => x.OvoNumber).IsUnique();
            b.HasIndex(x => x.Name).IsClustered();
        }
    }

    public class OrganisationTerminationListItemView :
        Projection<OrganisationDetailItemView>,
        IEventHandler<OrganisationTerminationFoundInKbo>,
        IEventHandler<OrganisationCouplingWithKboTerminated>,
        IEventHandler<OrganisationCouplingWithKboCancelled>
    {
        private readonly IMemoryCaches _memoryCaches;
        private readonly IEventStore _eventStore;
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationTerminationList
        }

        public OrganisationTerminationListItemView(
            ILogger<OrganisationDetailItemView> logger,
            IMemoryCaches memoryCaches,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _memoryCaches = memoryCaches;
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<OrganisationTerminationFoundInKbo> message)
        {
            var organisationName = _memoryCaches.OrganisationNames[message.Body.OrganisationId];
            var organisationOvo = _memoryCaches.OvoNumbers[message.Body.OrganisationId];
            var organisationListItem = new OrganisationTerminationListItem
            {
                Id = message.Body.OrganisationId,
                Name = organisationName,
                OvoNumber = organisationOvo,
                KboNumber = message.Body.KboNumber,
                Status = TerminationStatus.Proposed,
                Date = message.Body.TerminationDate,
                Code = message.Body.TerminationCode,
                Reason = message.Body.TerminationReason,
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.OrganisationTerminationList.AddAsync(organisationListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<OrganisationCouplingWithKboTerminated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationTerminationListItem = await context.OrganisationTerminationList.SingleAsync(item =>
                    item.Id == message.Body.OrganisationId && item.KboNumber == message.Body.KboNumber);

                context.OrganisationTerminationList.Remove(organisationTerminationListItem);

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationTerminationListItem = await context.OrganisationTerminationList.SingleAsync(item =>
                    item.Id == message.Body.OrganisationId && item.KboNumber == message.Body.PreviousKboNumber);

                context.OrganisationTerminationList.Remove(organisationTerminationListItem);

                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }

    public enum TerminationStatus
    {
        None = 0,
        Proposed = 1
    }
}
