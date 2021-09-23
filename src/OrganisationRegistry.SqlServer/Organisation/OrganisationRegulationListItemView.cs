namespace OrganisationRegistry.SqlServer.Organisation
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
    using RegulationType;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.RegulationType.Events;

    public class OrganisationRegulationListItem
    {
        public Guid OrganisationRegulationId { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid? RegulationTypeId { get; set; }
        public string? RegulationTypeName { get; set; }
        public DateTime? Date { get; set; }
        public string? Link { get; set; }
        public string? Description { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class OrganisationRegulationListConfiguration : EntityMappingConfiguration<OrganisationRegulationListItem>
    {
        public const int RegulationLinkLength = 500;

        public override void Map(EntityTypeBuilder<OrganisationRegulationListItem> b)
        {
            b.ToTable(nameof(OrganisationRegulationListView.ProjectionTables.OrganisationRegulationList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.OrganisationRegulationId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.RegulationTypeId);
            b.Property(p => p.RegulationTypeName).HasMaxLength(RegulationTypeListConfiguration.NameLength);

            b.Property(p => p.Date);
            b.Property(p => p.Link).HasMaxLength(RegulationLinkLength);
            b.Property(p => p.Description);

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.RegulationTypeName).IsClustered();
            b.HasIndex(x => x.Link);
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class OrganisationRegulationListView :
        Projection<OrganisationRegulationListView>,
        IEventHandler<OrganisationRegulationAdded>,
        IEventHandler<OrganisationRegulationUpdated>,
        IEventHandler<RegulationTypeUpdated>,
        IEventHandler<OrganisationTerminated>,
        IEventHandler<OrganisationTerminatedV2>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationRegulationList
        }

        private readonly IEventStore _eventStore;

        public OrganisationRegulationListView(
            ILogger<OrganisationRegulationListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RegulationTypeUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var organisationRegulations = context.OrganisationRegulationList.Where(x => x.RegulationTypeId == message.Body.RegulationTypeId);
            if (!await organisationRegulations.AnyAsync())
                return;

            foreach (var organisationRegulation in organisationRegulations)
                organisationRegulation.RegulationTypeName = message.Body.Name;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRegulationAdded> message)
        {
            var organisationRegulationListItem = new OrganisationRegulationListItem
            {
                OrganisationRegulationId = message.Body.OrganisationRegulationId,
                OrganisationId = message.Body.OrganisationId,
                RegulationTypeId = message.Body.RegulationTypeId,
                Link = message.Body.Link,
                Date = message.Body.Date,
                Description = message.Body.Description,
                RegulationTypeName = message.Body.RegulationTypeName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            await context.OrganisationRegulationList.AddAsync(organisationRegulationListItem);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRegulationUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var regulation = await context.OrganisationRegulationList.SingleAsync(item => item.OrganisationRegulationId == message.Body.OrganisationRegulationId);

            regulation.OrganisationRegulationId = message.Body.OrganisationRegulationId;
            regulation.OrganisationId = message.Body.OrganisationId;
            regulation.RegulationTypeId = message.Body.RegulationTypeId;
            regulation.Link = message.Body.Link;
            regulation.Date = message.Body.Date;
            regulation.Description = message.Body.Description;
            regulation.RegulationTypeName = message.Body.RegulationTypeName;
            regulation.ValidFrom = message.Body.ValidFrom;
            regulation.ValidTo = message.Body.ValidTo;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            if (message.Body.FieldsToTerminate.Regulations == null ||
                !message.Body.FieldsToTerminate.Regulations.Any())
                return;

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var regulations = context.OrganisationRegulationList.Where(item =>
                message.Body.FieldsToTerminate.Regulations.Keys.Contains(item.OrganisationRegulationId));

            foreach (var regulation in regulations)
                regulation.ValidTo = message.Body.FieldsToTerminate.Regulations[regulation.OrganisationRegulationId];

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
        {
            if (message.Body.FieldsToTerminate.Regulations == null ||
                !message.Body.FieldsToTerminate.Regulations.Any())
                return;

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var regulations = context.OrganisationRegulationList.Where(item =>
                message.Body.FieldsToTerminate.Regulations.Keys.Contains(item.OrganisationRegulationId));

            foreach (var regulation in regulations)
                regulation.ValidTo = message.Body.FieldsToTerminate.Regulations[regulation.OrganisationRegulationId];

            await context.SaveChangesAsync();
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
