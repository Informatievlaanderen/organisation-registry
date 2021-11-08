namespace OrganisationRegistry.SqlServer.RegulationSubTheme
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using RegulationTheme;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.RegulationSubTheme.Events;
    using OrganisationRegistry.RegulationTheme.Events;

    public class RegulationSubThemeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid RegulationThemeId { get; set; }
        public string RegulationThemeName { get; set; }
    }

    public class RegulationSubThemeListConfiguration : EntityMappingConfiguration<RegulationSubThemeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<RegulationSubThemeListItem> b)
        {
            b.ToTable(nameof(RegulationSubThemeListView.ProjectionTables.RegulationSubThemeList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.Property(p => p.RegulationThemeId).IsRequired();
            b.Property(p => p.RegulationThemeName)
                .HasMaxLength(RegulationThemeListConfiguration.NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsClustered();
            b.HasIndex(x => x.RegulationThemeName);

            b.HasIndex(x => new { x.Name, x.RegulationThemeId }).IsUnique();
        }
    }

    public class RegulationSubThemeListView :
        Projection<RegulationSubThemeListView>,
        IEventHandler<RegulationSubThemeCreated>,
        IEventHandler<RegulationSubThemeUpdated>,
        IEventHandler<RegulationThemeUpdated>
    {
        protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
        public override string Schema => WellknownSchemas.BackofficeSchema;

        public enum ProjectionTables
        {
            RegulationSubThemeList
        }

        private readonly IEventStore _eventStore;

        public RegulationSubThemeListView(
            ILogger<RegulationSubThemeListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RegulationSubThemeCreated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var regulationSubTheme = new RegulationSubThemeListItem
            {
                Id = message.Body.RegulationSubThemeId,
                Name = message.Body.Name,
                RegulationThemeId = message.Body.RegulationThemeId,
                RegulationThemeName = message.Body.RegulationThemeName
            };

            await context.RegulationSubThemeList.AddAsync(regulationSubTheme);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RegulationSubThemeUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var regulationSubTheme = context.RegulationSubThemeList.SingleOrDefault(x => x.Id == message.Body.RegulationSubThemeId);
            if (regulationSubTheme == null)
                return; // TODO: Error?

            regulationSubTheme.Name = message.Body.Name;
            regulationSubTheme.RegulationThemeId = message.Body.RegulationThemeId;
            regulationSubTheme.RegulationThemeName = message.Body.RegulationThemeName;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RegulationThemeUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var regulationSubThemes = context.RegulationSubThemeList.Where(x => x.RegulationThemeId == message.Body.RegulationThemeId);
            if (!regulationSubThemes.Any())
                return;

            foreach (var regulationSubTheme in regulationSubThemes)
                regulationSubTheme.RegulationThemeName = message.Body.Name;

            await context.SaveChangesAsync();
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
