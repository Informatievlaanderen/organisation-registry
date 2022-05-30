namespace OrganisationRegistry.SqlServer.BodyClassificationType;

using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using OrganisationRegistry.BodyClassificationType.Events;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Events;

public class BodyClassificationTypeListItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}

public class BodyClassificationTypeListConfiguration : EntityMappingConfiguration<BodyClassificationTypeListItem>
{
    public const int NameLength = 500;

    public override void Map(EntityTypeBuilder<BodyClassificationTypeListItem> b)
    {
        b.ToTable(nameof(BodyClassificationTypeListView.ProjectionTables.BodyClassificationTypeList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.Id)
            .IsClustered(false);

        b.Property(p => p.Name)
            .HasMaxLength(NameLength)
            .IsRequired();

        b.HasIndex(x => x.Name).IsUnique().IsClustered();
    }
}

public class BodyClassificationTypeListView :
    Projection<BodyClassificationTypeListView>,
    IEventHandler<BodyClassificationTypeCreated>,
    IEventHandler<BodyClassificationTypeUpdated>
{
    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        BodyClassificationTypeList
    }

    private readonly IEventStore _eventStore;

    public BodyClassificationTypeListView(
        ILogger<BodyClassificationTypeListView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClassificationTypeCreated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var bodyClassificationType = new BodyClassificationTypeListItem
            {
                Id = message.Body.BodyClassificationTypeId,
                Name = message.Body.Name,
            };

            await context.BodyClassificationTypeList.AddAsync(bodyClassificationType);
            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClassificationTypeUpdated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var bodyClassificationType = context.BodyClassificationTypeList.SingleOrDefault(x => x.Id == message.Body.BodyClassificationTypeId);
            if (bodyClassificationType == null)
                return; // TODO: Error?

            bodyClassificationType.Name = message.Body.Name;
            await context.SaveChangesAsync();
        }
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}