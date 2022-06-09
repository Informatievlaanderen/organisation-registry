namespace OrganisationRegistry.SqlServer.Organisation;

using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation.Events;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Purpose.Events;
using OrganisationRegistry.Security;
using RebuildProjection = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

public class OrganisationDetailItem
{
    public Guid Id { get; set; }

    public string OvoNumber { get; set; } = null!;

    public string? KboNumber { get; set; }

    public string Name { get; set; } = null!;
    public string? ShortName { get; set; }
    public string? Article { get; set; }

    public string? ParentOrganisation { get; set; }
    public Guid? ParentOrganisationId { get; set; }

    /// <summary>
    /// The relationship the ParentOrganisation is in
    /// </summary>
    public Guid? ParentOrganisationOrganisationParentId { get; set; }

    public Guid? FormalFrameworkId { get; set; }
    public Guid? OrganisationClassificationId { get; set; }
    public Guid? OrganisationClassificationTypeId { get; set; }
    public string? Description { get; set; }

    public string? PurposeIds { get; set; }
    public string? PurposeNames { get; set; }

    public bool ShowOnVlaamseOverheidSites { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    public DateTime? OperationalValidFrom { get; set; }
    public DateTime? OperationalValidTo { get; set; }

    public bool IsTerminated { get; set; }
    public bool UnderVlimpersManagement { get; set; }

    public Guid? SourceId { get; set; }
    public string? SourceType { get; set; }
    public string? SourceOrganisationIdentifier { get; set; }
}

public class OrganisationDetailConfiguration : EntityMappingConfiguration<OrganisationDetailItem>
{
    public override void Map(EntityTypeBuilder<OrganisationDetailItem> b)
    {
        b.ToTable(nameof(OrganisationDetailItemView.ProjectionTables.OrganisationDetail), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.Id)
            .IsClustered(false);

        b.Property(p => p.OvoNumber).HasMaxLength(OrganisationListConfiguration.OvoNumberLength).IsRequired();

        b.Property(p => p.Name).HasMaxLength(OrganisationListConfiguration.NameLength).IsRequired();
        b.Property(p => p.ShortName);
        b.Property(p => p.Article).HasMaxLength(3);

        b.Property(p => p.ParentOrganisation);
        b.Property(p => p.ParentOrganisationId);
        b.Property(p => p.ParentOrganisationOrganisationParentId);

        b.Property(p => p.FormalFrameworkId);
        b.Property(p => p.OrganisationClassificationId);
        b.Property(p => p.OrganisationClassificationTypeId);
        b.Property(p => p.Description);

        b.Property(p => p.PurposeIds);
        b.Property(p => p.PurposeNames);

        b.Property(p => p.ValidFrom);
        b.Property(p => p.ValidTo);

        b.Property(p => p.IsTerminated);
        b.Property(p => p.UnderVlimpersManagement);

        b.HasIndex(x => x.OvoNumber).IsUnique();
        b.HasIndex(x => x.Name).IsClustered();
        b.HasIndex(x => x.ParentOrganisation);

        b.Property(p => p.SourceId);
        b.Property(p => p.SourceType);
        b.Property(p => p.SourceOrganisationIdentifier);
    }
}

public class OrganisationDetailItemView :
    Projection<OrganisationDetailItemView>,
    IEventHandler<OrganisationCreated>,
    IEventHandler<OrganisationCreatedFromKbo>,
    IEventHandler<OrganisationCoupledWithKbo>,
    IEventHandler<OrganisationCouplingWithKboCancelled>,
    IEventHandler<OrganisationTerminationSyncedWithKbo>,
    IEventHandler<OrganisationInfoUpdated>,
    IEventHandler<OrganisationNameUpdated>,
    IEventHandler<OrganisationArticleUpdated>,
    IEventHandler<OrganisationShortNameUpdated>,
    IEventHandler<OrganisationValidityUpdated>,
    IEventHandler<OrganisationOperationalValidityUpdated>,
    IEventHandler<OrganisationShowOnVlaamseOverheidSitesUpdated>,
    IEventHandler<OrganisationDescriptionUpdated>,
    IEventHandler<OrganisationPurposesUpdated>,
    IEventHandler<OrganisationInfoUpdatedFromKbo>,
    IEventHandler<OrganisationParentUpdated>,
    IEventHandler<ParentAssignedToOrganisation>,
    IEventHandler<ParentClearedFromOrganisation>,
    IEventHandler<PurposeUpdated>,
    IEventHandler<OrganisationTerminated>,
    IEventHandler<OrganisationTerminatedV2>,
    IEventHandler<OrganisationPlacedUnderVlimpersManagement>,
    IEventHandler<OrganisationReleasedFromVlimpersManagement>

{
    private readonly IEventStore _eventStore;
    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        OrganisationDetail
    }

    public OrganisationDetailItemView(
        ILogger<OrganisationDetailItemView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationCreated> message)
    {
        var organisationListItem = new OrganisationDetailItem
        {
            Id = message.Body.OrganisationId,
            Name = message.Body.Name,
            ShortName = message.Body.ShortName,
            Article = message.Body.Article,
            OvoNumber = message.Body.OvoNumber,
            Description = message.Body.Description,
            PurposeIds = message.Body.Purposes.ToSeparatedList("|", x => x.Id.ToString()),
            PurposeNames = message.Body.Purposes.OrderBy(x => x.Name).ToSeparatedList("|", x => x.Name),
            ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites,
            ValidFrom = message.Body.ValidFrom,
            ValidTo = message.Body.ValidTo,
            OperationalValidFrom = message.Body.OperationalValidFrom,
            OperationalValidTo = message.Body.OperationalValidTo,
            SourceId = message.Body.SourceId,
            SourceType = message.Body.SourceType,
            SourceOrganisationIdentifier = message.Body.SourceOrganisationIdentifier,
        };

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        await context.OrganisationDetail.AddAsync(organisationListItem);
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationCreatedFromKbo> message)
    {
        var organisationListItem = new OrganisationDetailItem
        {
            Id = message.Body.OrganisationId,
            Name = message.Body.Name,
            ShortName = message.Body.ShortName,
            Article = message.Body.Article,
            OvoNumber = message.Body.OvoNumber,
            Description = message.Body.Description,
            PurposeIds = message.Body.Purposes.ToSeparatedList("|", x => x.Id.ToString()),
            PurposeNames = message.Body.Purposes.OrderBy(x => x.Name).ToSeparatedList("|", x => x.Name),
            ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites,
            ValidFrom = message.Body.ValidFrom,
            ValidTo = message.Body.ValidTo
        };

        organisationListItem.KboNumber = message.Body.KboNumber;

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        await context.OrganisationDetail.AddAsync(organisationListItem);
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationCoupledWithKbo> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.KboNumber = message.Body.KboNumber;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationCouplingWithKboCancelled> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.KboNumber = null;

        organisationListItem.Name = message.Body.NameBeforeKboCoupling;
        organisationListItem.ShortName = message.Body.ShortNameBeforeKboCoupling;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationTerminationSyncedWithKbo> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.KboNumber = null;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationInfoUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.Name = message.Body.Name;
        organisationListItem.ShortName = message.Body.ShortName;
        organisationListItem.Article = message.Body.Article;
        organisationListItem.Description = message.Body.Description;
        organisationListItem.ValidFrom = message.Body.ValidFrom;
        organisationListItem.ValidTo = message.Body.ValidTo;
        organisationListItem.OperationalValidFrom = message.Body.OperationalValidFrom;
        organisationListItem.OperationalValidTo = message.Body.OperationalValidTo;
        organisationListItem.PurposeIds = message.Body.Purposes.ToSeparatedList("|", x => x.Id.ToString());
        organisationListItem.PurposeNames =
            message.Body.Purposes.OrderBy(x => x.Name).ToSeparatedList("|", x => x.Name);
        organisationListItem.ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites;

        foreach (var child in context.OrganisationDetail.Where(item =>
                     item.ParentOrganisationId == message.Body.OrganisationId))
            child.ParentOrganisation = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationNameUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.Name = message.Body.Name;

        foreach (var child in context.OrganisationDetail.Where(item =>
                     item.ParentOrganisationId == message.Body.OrganisationId))
            child.ParentOrganisation = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationArticleUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.Article = message.Body.Article;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationShortNameUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.ShortName = message.Body.ShortName;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationDescriptionUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.Description = message.Body.Description;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationShowOnVlaamseOverheidSitesUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationValidityUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.ValidFrom = message.Body.ValidFrom;
        organisationListItem.ValidTo = message.Body.ValidTo;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationOperationalValidityUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.OperationalValidFrom = message.Body.OperationalValidFrom;
        organisationListItem.OperationalValidTo = message.Body.OperationalValidTo;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationPurposesUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.PurposeIds = message.Body.Purposes.ToSeparatedList("|", x => x.Id.ToString());
        organisationListItem.PurposeNames =
            message.Body.Purposes.OrderBy(x => x.Name).ToSeparatedList("|", x => x.Name);

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationInfoUpdatedFromKbo> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.Name = message.Body.Name;
        organisationListItem.ShortName = message.Body.ShortName;

        foreach (var child in context.OrganisationDetail.Where(item =>
                     item.ParentOrganisationId == message.Body.OrganisationId))
            child.ParentOrganisation = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<PurposeUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItems = context.OrganisationDetail.Where(item => item.PurposeIds != null && item.PurposeIds.Contains(message.Body.PurposeId.ToString()));

        var previousPurposeName = message.Body.PreviousName;
        foreach (var organisationListItem in organisationListItems)
        {
            var currentNames = string.IsNullOrWhiteSpace(organisationListItem.PurposeNames)
                ? new List<string>()
                : organisationListItem.PurposeNames.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

            currentNames.Remove(previousPurposeName);
            currentNames.Add(message.Body.Name);

            organisationListItem.PurposeNames = currentNames.OrderBy(x => x).ToSeparatedList("|", x => x);
        }

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationParentUpdated> message)
    {
        if (!message.Body.PreviousParentOrganisationId.HasValue)
            return;

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        if (organisationListItem.ParentOrganisationOrganisationParentId !=
            message.Body.OrganisationOrganisationParentId)
            return;

        organisationListItem.ParentOrganisationId = message.Body.ParentOrganisationId;
        organisationListItem.ParentOrganisation = (await context.OrganisationDetail
            .SingleAsync(item => item.Id == message.Body.ParentOrganisationId)).Name;
        organisationListItem.ParentOrganisationOrganisationParentId =
            message.Body.OrganisationOrganisationParentId;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<ParentAssignedToOrganisation> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.ParentOrganisationId = message.Body.ParentOrganisationId;
        organisationListItem.ParentOrganisation = (await context.OrganisationDetail
            .SingleAsync(item => item.Id == message.Body.ParentOrganisationId)).Name;
        organisationListItem.ParentOrganisationOrganisationParentId =
            message.Body.OrganisationOrganisationParentId;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<ParentClearedFromOrganisation> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.ParentOrganisationId = null;
        organisationListItem.ParentOrganisation = null;
        organisationListItem.ParentOrganisationOrganisationParentId = null;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.IsTerminated = true;

        if (message.Body.FieldsToTerminate.OrganisationValidity.HasValue)
        {
            organisationListItem.ValidTo = message.Body.FieldsToTerminate.OrganisationValidity;
            organisationListItem.OperationalValidTo = message.Body.FieldsToTerminate.OrganisationValidity;
        }

        if(message.Body.ForcedKboTermination)
            organisationListItem.KboNumber = null;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.IsTerminated = true;

        if (message.Body.FieldsToTerminate.OrganisationValidity.HasValue)
        {
            organisationListItem.ValidTo = message.Body.FieldsToTerminate.OrganisationValidity;
            organisationListItem.OperationalValidTo = message.Body.FieldsToTerminate.OrganisationValidity;
        }

        if(message.Body.ForcedKboTermination)
            organisationListItem.KboNumber = null;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationPlacedUnderVlimpersManagement> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.UnderVlimpersManagement = true;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationReleasedFromVlimpersManagement> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem =
            await context.OrganisationDetail.SingleAsync(item => item.Id == message.Body.OrganisationId);

        organisationListItem.UnderVlimpersManagement = false;

        await context.SaveChangesAsync();
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}
