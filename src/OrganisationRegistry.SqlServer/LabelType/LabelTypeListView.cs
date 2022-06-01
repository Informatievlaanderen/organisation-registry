namespace OrganisationRegistry.SqlServer.LabelType;

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
using OrganisationRegistry.LabelType.Events;

public class LabelTypeListItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}

public class LabelTypeListConfiguration : EntityMappingConfiguration<LabelTypeListItem>
{
    public const int NameLength = 500;

    public override void Map(EntityTypeBuilder<LabelTypeListItem> b)
    {
        b.ToTable(nameof(LabelTypeListView.ProjectionTables.LabelTypeList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.Id)
            .IsClustered(false);

        b.Property(p => p.Name)
            .HasMaxLength(NameLength)
            .IsRequired();

        b.HasIndex(x => x.Name).IsUnique().IsClustered();
    }
}

public class LabelTypeListView :
    Projection<LabelTypeListView>,
    IEventHandler<LabelTypeCreated>,
    IEventHandler<LabelTypeUpdated>
{
    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        LabelTypeList
    }

    private readonly IEventStore _eventStore;

    public LabelTypeListView(
        ILogger<LabelTypeListView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LabelTypeCreated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var labelType = new LabelTypeListItem
            {
                Id = message.Body.LabelTypeId,
                Name = message.Body.Name,
            };

            await context.LabelTypeList.AddAsync(labelType);
            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LabelTypeUpdated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var labelType = context.LabelTypeList.SingleOrDefault(x => x.Id == message.Body.LabelTypeId);
            if (labelType == null)
                return; // TODO: Error?

            labelType.Name = message.Body.Name;
            await context.SaveChangesAsync();
        }
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}
