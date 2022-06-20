namespace OrganisationRegistry.SqlServer.RegulationTheme;

using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.RegulationTheme.Events;

public class RegulationThemeListItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}

public class RegulationThemeListConfiguration : EntityMappingConfiguration<RegulationThemeListItem>
{
    public const int NameLength = 500;

    public override void Map(EntityTypeBuilder<RegulationThemeListItem> b)
    {
        b.ToTable(nameof(RegulationThemeListView.ProjectionTables.RegulationThemeList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.Id)
            .IsClustered(false);

        b.Property(p => p.Name)
            .HasMaxLength(NameLength)
            .IsRequired();

        b.HasIndex(x => x.Name).IsUnique().IsClustered();
    }
}

public class RegulationThemeListView :
    Projection<RegulationThemeListView>,
    IEventHandler<RegulationThemeCreated>,
    IEventHandler<RegulationThemeUpdated>
{
    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        RegulationThemeList,
    }

    private readonly IEventStore _eventStore;

    public RegulationThemeListView(
        ILogger<RegulationThemeListView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RegulationThemeCreated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var regulationTheme = new RegulationThemeListItem
        {
            Id = message.Body.RegulationThemeId,
            Name = message.Body.Name,
        };

        await context.RegulationThemeList.AddAsync(regulationTheme);
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RegulationThemeUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var regulationTheme = context.RegulationThemeList.SingleOrDefault(x => x.Id == message.Body.RegulationThemeId);
        if (regulationTheme == null)
            return; // TODO: Error?

        regulationTheme.Name = message.Body.Name;
        await context.SaveChangesAsync();
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}
