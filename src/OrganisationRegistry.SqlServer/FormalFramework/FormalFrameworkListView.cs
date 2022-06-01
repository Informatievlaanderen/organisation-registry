namespace OrganisationRegistry.SqlServer.FormalFramework;

using System;
using System.Linq;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.FormalFramework.Events;
using OrganisationRegistry.FormalFrameworkCategory.Events;
using OrganisationRegistry.Infrastructure;

public class FormalFrameworkListItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public Guid FormalFrameworkCategoryId { get; set; }

    public string FormalFrameworkCategoryName { get; set; } = null!;
}

public class FormalFrameworkListConfiguration : EntityMappingConfiguration<FormalFrameworkListItem>
{
    public const int NameLength = 500;
    public const int CodeLength = 50;
    public const int FormalFrameworkCategoryNameLength = 500;

    public override void Map(EntityTypeBuilder<FormalFrameworkListItem> b)
    {
        b.ToTable(nameof(FormalFrameworkListView.ProjectionTables.FormalFrameworkList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.Id)
            .IsClustered(false);

        b.Property(p => p.Name)
            .HasMaxLength(NameLength)
            .IsRequired();

        b.Property(p => p.Code)
            .HasMaxLength(CodeLength)
            .IsRequired();

        b.Property(p => p.FormalFrameworkCategoryId)
            .IsRequired();

        b.Property(p => p.FormalFrameworkCategoryName)
            .HasMaxLength(FormalFrameworkCategoryNameLength)
            .IsRequired();

        b.HasIndex(x => x.Name).IsClustered();
        b.HasIndex(x => x.Code);
        b.HasIndex(x => x.FormalFrameworkCategoryName);

        b.HasIndex(x => new { x.Name, x.FormalFrameworkCategoryId }).IsUnique();
    }
}

public class FormalFrameworkListView :
    Projection<FormalFrameworkListView>,
    IEventHandler<FormalFrameworkCreated>,
    IEventHandler<FormalFrameworkUpdated>,
    IEventHandler<FormalFrameworkCategoryUpdated>
{
    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        FormalFrameworkList
    }

    private readonly IEventStore _eventStore;
    public FormalFrameworkListView(
        ILogger<FormalFrameworkListView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkCreated> message)
    {
        var formalFramework = new FormalFrameworkListItem
        {
            Id = message.Body.FormalFrameworkId,
            Name = message.Body.Name,
            Code = message.Body.Code,
            FormalFrameworkCategoryId = message.Body.FormalFrameworkCategoryId,
            FormalFrameworkCategoryName = message.Body.FormalFrameworkCategoryName
        };

        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            await context.FormalFrameworkList.AddAsync(formalFramework);
            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkUpdated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var formalFramework = context.FormalFrameworkList.SingleOrDefault(x => x.Id == message.Body.FormalFrameworkId);
            if (formalFramework == null)
                return; // TODO: Error?

            formalFramework.Name = message.Body.Name;
            formalFramework.Code = message.Body.Code;
            formalFramework.FormalFrameworkCategoryId = message.Body.FormalFrameworkCategoryId;
            formalFramework.FormalFrameworkCategoryName = message.Body.FormalFrameworkCategoryName;

            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkCategoryUpdated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var formalFrameworks = context.FormalFrameworkList.Where(x => x.FormalFrameworkCategoryId == message.Body.FormalFrameworkCategoryId);
            if (!formalFrameworks.Any())
                return;

            foreach (var formalFramework in formalFrameworks)
                formalFramework.FormalFrameworkCategoryName = message.Body.Name;

            await context.SaveChangesAsync();
        }
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}
