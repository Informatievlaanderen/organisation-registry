namespace OrganisationRegistry.SqlServer.Body;

using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Body.Events;

using System.Linq;
using System.Threading.Tasks;
using FormalFramework;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.FormalFramework.Events;
using OrganisationRegistry.Infrastructure;

public class BodyFormalFrameworkListItem
{
    public Guid BodyFormalFrameworkId { get; set; }
    public Guid BodyId { get; set; }

    public Guid FormalFrameworkId { get; set; }
    public string FormalFrameworkName { get; set; } = null!;

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class BodyFormalFrameworkListConfiguration : EntityMappingConfiguration<BodyFormalFrameworkListItem>
{
    public override void Map(EntityTypeBuilder<BodyFormalFrameworkListItem> b)
    {
        b.ToTable(nameof(BodyFormalFrameworkListView.ProjectionTables.BodyFormalFrameworkList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.BodyFormalFrameworkId)
            .IsClustered(false);

        b.Property(p => p.BodyId).IsRequired();

        b.Property(p => p.FormalFrameworkId).IsRequired();
        b.Property(p => p.FormalFrameworkName).HasMaxLength(FormalFrameworkListConfiguration.NameLength).IsRequired();

        b.Property(p => p.ValidFrom);
        b.Property(p => p.ValidTo);

        b.HasIndex(x => x.FormalFrameworkName).IsClustered();
        b.HasIndex(x => x.ValidFrom);
        b.HasIndex(x => x.ValidTo);
    }
}

public class BodyFormalFrameworkListView :
    Projection<BodyFormalFrameworkListView>,
    IEventHandler<BodyFormalFrameworkAdded>,
    IEventHandler<BodyFormalFrameworkUpdated>,
    IEventHandler<FormalFrameworkUpdated>
{
    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        BodyFormalFrameworkList,
    }

    private readonly IEventStore _eventStore;

    public BodyFormalFrameworkListView(
        ILogger<BodyFormalFrameworkListView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkUpdated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var bodyFormalFrameworks = context.BodyFormalFrameworkList.Where(x => x.FormalFrameworkId == message.Body.FormalFrameworkId);
            if (!bodyFormalFrameworks.Any())
                return;

            foreach (var bodyFormalFramework in bodyFormalFrameworks)
                bodyFormalFramework.FormalFrameworkName = message.Body.Name;

            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyFormalFrameworkAdded> message)
    {
        var bodyFormalFrameworkListItem = new BodyFormalFrameworkListItem
        {
            BodyFormalFrameworkId = message.Body.BodyFormalFrameworkId,
            BodyId = message.Body.BodyId,
            FormalFrameworkId = message.Body.FormalFrameworkId,
            FormalFrameworkName = message.Body.FormalFrameworkName,
            ValidFrom = message.Body.ValidFrom,
            ValidTo = message.Body.ValidTo,
        };

        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            await context.BodyFormalFrameworkList.AddAsync(bodyFormalFrameworkListItem);
            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyFormalFrameworkUpdated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var bodyFormalFramework = await context.BodyFormalFrameworkList.SingleAsync(item => item.BodyFormalFrameworkId == message.Body.BodyFormalFrameworkId);

            bodyFormalFramework.BodyFormalFrameworkId = message.Body.BodyFormalFrameworkId;
            bodyFormalFramework.BodyId = message.Body.BodyId;
            bodyFormalFramework.FormalFrameworkId = message.Body.FormalFrameworkId;
            bodyFormalFramework.FormalFrameworkName = message.Body.FormalFrameworkName;
            bodyFormalFramework.ValidFrom = message.Body.ValidFrom;
            bodyFormalFramework.ValidTo = message.Body.ValidTo;

            await context.SaveChangesAsync();
        }
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}
