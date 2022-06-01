namespace OrganisationRegistry.SqlServer.Organisation;

using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure;
using OrganisationRegistry.Infrastructure.Events;
using System.Linq;
using System.Threading.Tasks;
using Body;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Body.Events;
using OrganisationRegistry.Infrastructure;

public class OrganisationBodyListItem
{
    public Guid OrganisationBodyId { get; set; }
    public Guid OrganisationId { get; set; }
    public Guid BodyId { get; set; }
    public string BodyName { get; set; } = null!;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class OrganisationBodyListConfiguration : EntityMappingConfiguration<OrganisationBodyListItem>
{
    public override void Map(EntityTypeBuilder<OrganisationBodyListItem> b)
    {
        b.ToTable(nameof(OrganisationBodyListItemView.ProjectionTables.OrganisationBodyList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.OrganisationBodyId)
            .IsClustered(false);

        b.Property(p => p.OrganisationId).IsRequired();

        b.Property(p => p.BodyId).IsRequired();
        b.Property(p => p.BodyName).HasMaxLength(BodyListConfiguration.NameLength).IsRequired();

        b.Property(p => p.ValidFrom);
        b.Property(p => p.ValidTo);

        b.HasIndex(x => x.BodyName).IsClustered();
        b.HasIndex(x => x.ValidFrom);
        b.HasIndex(x => x.ValidTo);
    }
}

public class OrganisationBodyListItemView :
    Projection<OrganisationBodyListItemView>,
    IEventHandler<BodyInfoChanged>,
    IEventHandler<BodyOrganisationAdded>,
    IEventHandler<BodyOrganisationUpdated>
{
    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        OrganisationBodyList
    }

    private readonly IEventStore _eventStore;
    public OrganisationBodyListItemView(
        ILogger<OrganisationBodyListItemView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }
    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyInfoChanged> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var organisationBodies = context.OrganisationBodyList.Where(x => x.BodyId == message.Body.BodyId);
            if (!organisationBodies.Any())
                return;

            foreach (var organisationBody in organisationBodies)
                organisationBody.BodyName = message.Body.Name;

            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationAdded> message)
    {
        var organisationBodyListItem = new OrganisationBodyListItem
        {
            OrganisationBodyId = message.Body.BodyOrganisationId,
            OrganisationId = message.Body.OrganisationId,
            BodyId = message.Body.BodyId,
            BodyName = message.Body.BodyName,
            ValidFrom = message.Body.ValidFrom,
            ValidTo = message.Body.ValidTo
        };

        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            await context.OrganisationBodyList.AddAsync(organisationBodyListItem);
            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var organisation = await context.OrganisationBodyList.SingleAsync(item => item.OrganisationBodyId == message.Body.BodyOrganisationId);

            organisation.OrganisationBodyId = message.Body.BodyOrganisationId;
            organisation.OrganisationId = message.Body.OrganisationId;
            organisation.BodyId = message.Body.BodyId;
            organisation.ValidFrom = message.Body.ValidFrom;
            organisation.ValidTo = message.Body.ValidTo;

            await context.SaveChangesAsync();
        }
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}
