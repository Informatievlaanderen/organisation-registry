namespace OrganisationRegistry.SqlServer.OrganisationClassificationType;

using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.OrganisationClassificationType.Events;

public class OrganisationClassificationTypeListItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public bool AllowDifferentClassificationsToOverlap { get; set; }
}

public class OrganisationClassificationTypeListConfiguration : EntityMappingConfiguration<OrganisationClassificationTypeListItem>
{
    public const int NameLength = 500;

    public override void Map(EntityTypeBuilder<OrganisationClassificationTypeListItem> b)
    {
        b.ToTable(nameof(OrganisationClassificationTypeListView.ProjectionTables.OrganisationClassificationTypeList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.Id)
            .IsClustered(false);

        b.Property(p => p.Name)
            .HasMaxLength(NameLength)
            .IsRequired();

        b.HasIndex(x => x.Name).IsUnique().IsClustered();
    }
}

public class OrganisationClassificationTypeListView :
    Projection<OrganisationClassificationTypeListView>,
    IEventHandler<OrganisationClassificationTypeCreated>,
    IEventHandler<OrganisationClassificationTypeUpdated>,
    IEventHandler<OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged>
{
    protected override string[] ProjectionTableNames
        => Enum.GetNames(typeof(ProjectionTables));

    public override string Schema
        => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        OrganisationClassificationTypeList,
    }

    private readonly IEventStore _eventStore;

    public OrganisationClassificationTypeListView(
        ILogger<OrganisationClassificationTypeListView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationTypeCreated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);

        var organisationClassificationType = new OrganisationClassificationTypeListItem
        {
            Id = message.Body.OrganisationClassificationTypeId,
            Name = message.Body.Name,
            AllowDifferentClassificationsToOverlap = false,
        };

        context.OrganisationClassificationTypeList.Add(organisationClassificationType);
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationTypeUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);

        var organisationClassificationType = await context.OrganisationClassificationTypeList.SingleAsync(x => x.Id == message.Body.OrganisationClassificationTypeId);
        organisationClassificationType.Name = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);

        var organisationClassificationType = await context.OrganisationClassificationTypeList.SingleAsync(x => x.Id == message.Body.OrganisationClassificationTypeId);
        organisationClassificationType.AllowDifferentClassificationsToOverlap = message.Body.IsAllowed;

        await context.SaveChangesAsync();
    }
}
