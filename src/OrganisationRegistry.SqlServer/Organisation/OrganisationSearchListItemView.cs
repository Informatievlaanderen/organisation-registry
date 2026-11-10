namespace OrganisationRegistry.SqlServer.Organisation;

using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.OrganisationClassification.Events;
using RebuildProjection = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

public class OrganisationSearchListItem
{
    public int Id { get; set; }

    public Guid OrganisationId { get; set; }

    public string OvoNumber { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? ShortName { get; set; }

    public string? ParentOrganisation { get; set; }
    public Guid? ParentOrganisationId { get; set; }
    public string? ParentOrganisationOvoNumber { get; set; }

    /// <summary>
    /// The relationship the ParentOrganisation is in. This can be either an OrganisationOrganisationId or an OrganisationFormalFrameworkId.
    /// </summary>
    public Guid? ParentOrganisationsRelationshipId { get; set; }

    public Guid? FormalFrameworkId { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    public List<OrganisationSearchFormalFrameworkValidity> FormalFrameworkValidities { get; set; } = new();
    public List<OrganisationSearchClassificationValidity> OrganisationSearchClassificationValidities { get; set; } = new();
}

public class OrganisationSearchFormalFrameworkValidity
{
    public int Id { get; set; }
    public Guid OrganisationFormalFrameworkId { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class OrganisationSearchClassificationValidity
{
    public int Id { get; set; }
    public Guid OrganisationClassificationTypeId { get; set; }
    public Guid OrganisationClassificationId { get; set; }
    public Guid OrganisationOrganisationClassificationId { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class OrganisationSearchListConfiguration : EntityMappingConfiguration<OrganisationSearchListItem>
{
    private const int OvoNumberLength = OrganisationListConfiguration.OvoNumberLength;
    private const int NameLength = OrganisationListConfiguration.NameLength;

    public override void Map(EntityTypeBuilder<OrganisationSearchListItem> b)
    {
        b.ToTable(nameof(OrganisationSearchListItemView.ProjectionTables.OrganisationSearchList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.Id)
            .IsClustered(false);

        b.Property(p => p.Id).UseIdentityColumn();
        b.Property(p => p.OrganisationId).IsRequired();

        b.HasIndex("OrganisationId", "FormalFrameworkId").IsUnique();

        b.Property(p => p.OvoNumber).HasMaxLength(OvoNumberLength).IsRequired();

        b.Property(p => p.Name).HasMaxLength(NameLength).IsRequired();
        b.Property(p => p.ShortName);

        b.Property(p => p.ParentOrganisation).HasMaxLength(NameLength);
        b.Property(p => p.ParentOrganisationId);
        b.Property(p => p.ParentOrganisationOvoNumber);
        b.Property(p => p.ParentOrganisationsRelationshipId);

        b.Property(p => p.FormalFrameworkId).IsRequired(false);

        b.Property(p => p.ValidFrom);
        b.Property(p => p.ValidTo);

        b.HasMany(p => p.FormalFrameworkValidities).WithOne().OnDelete(DeleteBehavior.Cascade);
        b.HasMany(p => p.OrganisationSearchClassificationValidities).WithOne().OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.OvoNumber);
        b.HasIndex(x => x.Name).IsClustered();
        b.HasIndex(x => x.ShortName);
        b.HasIndex(x => x.ParentOrganisation);
        b.HasIndex(x => x.FormalFrameworkId);
        b.HasIndex(x => x.ValidFrom);
        b.HasIndex(x => x.ValidTo);
    }
}

public class OrganisationSearchFormalFrameworkValidityListConfiguration : EntityMappingConfiguration<OrganisationSearchFormalFrameworkValidity>
{
    public override void Map(EntityTypeBuilder<OrganisationSearchFormalFrameworkValidity> b)
    {
        b.ToTable(nameof(OrganisationSearchListItemView.ProjectionTables.OrganisationSearchFormalFrameworkValidity), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.Id)
            .IsClustered();

        b.Property(p => p.Id).UseIdentityColumn();

        b.Property(p => p.OrganisationFormalFrameworkId);
        b.Property(p => p.ValidFrom);
        b.Property(p => p.ValidTo);

        b.HasIndex(x => x.ValidFrom);
        b.HasIndex(x => x.ValidTo);
    }
}

public class OrganisationSearchClassificationValidityListConfiguration : EntityMappingConfiguration<OrganisationSearchClassificationValidity>
{
    public override void Map(EntityTypeBuilder<OrganisationSearchClassificationValidity> b)
    {
        b.ToTable(nameof(OrganisationSearchListItemView.ProjectionTables.OrganisationSearchClassificationValidity), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.Id)
            .IsClustered(false);

        b.Property(p => p.Id).UseIdentityColumn();

        b.Property(p => p.OrganisationClassificationTypeId);
        b.Property(p => p.OrganisationClassificationId);
        b.Property(p => p.OrganisationOrganisationClassificationId);

        b.Property(p => p.ValidFrom);
        b.Property(p => p.ValidTo);

        b.HasIndex(x => x.OrganisationClassificationTypeId);
        b.HasIndex(x => x.ValidFrom);
        b.HasIndex(x => x.ValidTo);
    }
}

/// <summary>
/// Read model voor het zoeken op de hoofdpagina (OR-3785). Dit is een bewuste kopie van
/// <see cref="OrganisationListItemView"/>: de hoofdpagina heeft een eigen read model nodig om op
/// code (OVO-nummer, KBO-nummer, Orafin-sleutel) te kunnen zoeken, zonder het bestaande
/// OrganisationList-read model en zijn consumenten te raken.
/// <br />
/// Houd de eventverzameling van beide projecties gelijk; een architectuurtest waakt over drift.
/// </summary>
public class OrganisationSearchListItemView :
    Projection<OrganisationSearchListItemView>,
    IEventHandler<OrganisationCreated>,
    IEventHandler<OrganisationCreatedFromKbo>,
    IEventHandler<OrganisationInfoUpdated>,
    IEventHandler<OrganisationNameUpdated>,
    IEventHandler<OrganisationShortNameUpdated>,
    IEventHandler<OrganisationValidityUpdated>,
    IEventHandler<OrganisationInfoUpdatedFromKbo>,
    IEventHandler<OrganisationParentUpdated>,
    IEventHandler<ParentAssignedToOrganisation>,
    IEventHandler<ParentClearedFromOrganisation>,
    IEventHandler<FormalFrameworkAssignedToOrganisation>,
    IEventHandler<FormalFrameworkClearedFromOrganisation>,
    IEventHandler<OrganisationFormalFrameworkAdded>,
    IEventHandler<OrganisationFormalFrameworkUpdated>,
    IEventHandler<OrganisationFormalFrameworkRemoved>,
    IEventHandler<OrganisationOrganisationClassificationAdded>,
    IEventHandler<KboLegalFormOrganisationOrganisationClassificationAdded>,
    IEventHandler<KboLegalFormOrganisationOrganisationClassificationRemoved>,
    IEventHandler<OrganisationCouplingWithKboCancelled>,
    IEventHandler<OrganisationTerminationSyncedWithKbo>,
    IEventHandler<OrganisationOrganisationClassificationUpdated>,
    IEventHandler<OrganisationClassificationUpdated>,
    IEventHandler<OrganisationTerminated>,
    IEventHandler<OrganisationTerminatedV2>
{
    protected override string[] ProjectionTableNames
        => Enum.GetNames(typeof(ProjectionTables));

    public override string Schema
        => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        OrganisationSearchFormalFrameworkValidity,
        OrganisationSearchClassificationValidity,
        OrganisationSearchCodeList,
        OrganisationSearchList,
    }

    private readonly IEventStore _eventStore;

    public OrganisationSearchListItemView(
        ILogger<OrganisationSearchListItemView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }

    private static OrganisationSearchListItem GetParentOrganisation(OrganisationRegistryContext context, Guid parentOrganisationId)
    {
        return context.OrganisationSearchList.Single(item => item.OrganisationId == parentOrganisationId && item.FormalFrameworkId == null);
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreated> message)
    {
        var organisationListItem = new OrganisationSearchListItem
        {
            OrganisationId = message.Body.OrganisationId,
            Name = message.Body.Name,
            ShortName = message.Body.ShortName,
            OvoNumber = message.Body.OvoNumber,
            ValidFrom = message.Body.ValidFrom,
            ValidTo = message.Body.ValidTo,
        };

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        await context.OrganisationSearchList.AddAsync(organisationListItem);
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreatedFromKbo> message)
    {
        var organisationListItem = new OrganisationSearchListItem
        {
            OrganisationId = message.Body.OrganisationId,
            Name = message.Body.Name,
            ShortName = message.Body.ShortName,
            OvoNumber = message.Body.OvoNumber,
            ValidFrom = message.Body.ValidFrom,
            ValidTo = message.Body.ValidTo,
        };

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        await context.OrganisationSearchList.AddAsync(organisationListItem);
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        foreach (var organisationListItem in context.OrganisationSearchList.Where(item => item.OrganisationId == message.Body.OrganisationId))
        {
            organisationListItem.Name = message.Body.Name;
            organisationListItem.ShortName = message.Body.ShortName;
            organisationListItem.ValidFrom = message.Body.ValidFrom;
            organisationListItem.ValidTo = message.Body.ValidTo;
        }

        foreach (var child in context.OrganisationSearchList.Where(item => item.ParentOrganisationId == message.Body.OrganisationId))
        {
            child.ParentOrganisation = message.Body.Name;
            child.ParentOrganisationOvoNumber = message.Body.OvoNumber;
        }

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationNameUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisations = context.OrganisationSearchList.Where(item => item.OrganisationId == message.Body.OrganisationId);
        foreach (var organisationListItem in organisations)
        {
            organisationListItem.Name = message.Body.Name;
        }

        var ovoNumber = (await organisations.FirstAsync()).OvoNumber;

        foreach (var child in context.OrganisationSearchList.Where(item => item.ParentOrganisationId == message.Body.OrganisationId))
        {
            child.ParentOrganisation = message.Body.Name;
            child.ParentOrganisationOvoNumber = ovoNumber;
        }

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationShortNameUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        foreach (var organisationListItem in context.OrganisationSearchList.Where(item => item.OrganisationId == message.Body.OrganisationId))
        {
            organisationListItem.ShortName = message.Body.ShortName;
        }

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationValidityUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        foreach (var organisationListItem in context.OrganisationSearchList.Where(item => item.OrganisationId == message.Body.OrganisationId))
        {
            organisationListItem.ValidFrom = message.Body.ValidFrom;
            organisationListItem.ValidTo = message.Body.ValidTo;
        }

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        foreach (var organisationListItem in context.OrganisationSearchList.Where(item => item.OrganisationId == message.Body.OrganisationId))
        {
            organisationListItem.Name = message.Body.Name;
            organisationListItem.ShortName = message.Body.ShortName;
        }

        foreach (var child in context.OrganisationSearchList.Where(item => item.ParentOrganisationId == message.Body.OrganisationId))
            child.ParentOrganisation = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentUpdated> message)
    {
        if (!message.Body.PreviousParentOrganisationId.HasValue)
            return;

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem = context.OrganisationSearchList.SingleOrDefault(
            item =>
                item.OrganisationId == message.Body.OrganisationId &&
                item.ParentOrganisationsRelationshipId == message.Body.OrganisationOrganisationParentId &&
                item.FormalFrameworkId == null);

        if (organisationListItem == null)
            return;

        organisationListItem.ParentOrganisationId = message.Body.ParentOrganisationId;
        organisationListItem.ParentOrganisation = message.Body.ParentOrganisationName;
        var parentOrganisation = GetParentOrganisation(context, message.Body.ParentOrganisationId);
        organisationListItem.ParentOrganisationOvoNumber = parentOrganisation.OvoNumber;
        organisationListItem.ParentOrganisationsRelationshipId = message.Body.OrganisationOrganisationParentId;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ParentAssignedToOrganisation> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem = context.OrganisationSearchList.Single(
            item =>
                item.OrganisationId == message.Body.OrganisationId &&
                item.FormalFrameworkId == null);

        organisationListItem.ParentOrganisationId = message.Body.ParentOrganisationId;
        var parentOrganisation = GetParentOrganisation(context, message.Body.ParentOrganisationId);
        organisationListItem.ParentOrganisation = parentOrganisation.Name;
        organisationListItem.ParentOrganisationOvoNumber = parentOrganisation.OvoNumber;
        organisationListItem.ParentOrganisationsRelationshipId = message.Body.OrganisationOrganisationParentId;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ParentClearedFromOrganisation> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem = context.OrganisationSearchList.Single(
            item =>
                item.OrganisationId == message.Body.OrganisationId &&
                item.FormalFrameworkId == null);

        organisationListItem.ParentOrganisationId = null;
        organisationListItem.ParentOrganisation = null;
        organisationListItem.ParentOrganisationOvoNumber = null;
        organisationListItem.ParentOrganisationsRelationshipId = null;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkAssignedToOrganisation> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem = context.OrganisationSearchList.Single(
            item =>
                item.FormalFrameworkId == message.Body.FormalFrameworkId &&
                item.OrganisationId == message.Body.OrganisationId);

        organisationListItem.ParentOrganisationId = message.Body.ParentOrganisationId;
        var parentOrganisation = GetParentOrganisation(context, message.Body.ParentOrganisationId);
        organisationListItem.ParentOrganisation = parentOrganisation.Name;
        organisationListItem.ParentOrganisationOvoNumber = parentOrganisation.OvoNumber;
        organisationListItem.ParentOrganisationsRelationshipId = message.Body.OrganisationFormalFrameworkId;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkClearedFromOrganisation> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationListItem = context.OrganisationSearchList.Single(
            item =>
                item.FormalFrameworkId == message.Body.FormalFrameworkId &&
                item.OrganisationId == message.Body.OrganisationId);

        organisationListItem.ParentOrganisationId = null;
        organisationListItem.ParentOrganisation = null;
        organisationListItem.ParentOrganisationOvoNumber = null;
        organisationListItem.ParentOrganisationsRelationshipId = null;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkAdded> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        // CHILD STUFF
        if (!OrganisationExistsForFormalFramework(context, message.Body.OrganisationId, message.Body.FormalFrameworkId))
        {
            // copy regular organisation row to create child row
            var regularOrganisationSearchListItem =
                context.OrganisationSearchList
                    .Include(item => item.OrganisationSearchClassificationValidities)
                    .Single(
                        item =>
                            item.OrganisationId == message.Body.OrganisationId &&
                            item.FormalFrameworkId == null);

            var organisationListItemForFormalFramework = new OrganisationSearchListItem
            {
                OrganisationId = regularOrganisationSearchListItem.OrganisationId,
                Name = regularOrganisationSearchListItem.Name,
                ShortName = regularOrganisationSearchListItem.ShortName,
                OvoNumber = regularOrganisationSearchListItem.OvoNumber,
                ValidFrom = regularOrganisationSearchListItem.ValidFrom,
                ValidTo = regularOrganisationSearchListItem.ValidTo,
                OrganisationSearchClassificationValidities = regularOrganisationSearchListItem.OrganisationSearchClassificationValidities.Select(Copy).ToList(),
                FormalFrameworkId = message.Body.FormalFrameworkId,
                FormalFrameworkValidities = new List<OrganisationSearchFormalFrameworkValidity>
                {
                    new()
                    {
                        ValidFrom = message.Body.ValidFrom,
                        ValidTo = message.Body.ValidTo,
                        OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId,
                    },
                },
            };

            await context.OrganisationSearchList.AddAsync(organisationListItemForFormalFramework);
        }
        else
        {
            // update child row
            var organisationListItem =
                context.OrganisationSearchList
                    .Include(item => item.FormalFrameworkValidities)
                    .Single(
                        item =>
                            item.FormalFrameworkId == message.Body.FormalFrameworkId &&
                            item.OrganisationId == message.Body.OrganisationId);

            organisationListItem.ParentOrganisationId = message.Body.ParentOrganisationId;
            organisationListItem.ParentOrganisation = message.Body.ParentOrganisationName;
            var parentOrganisation = GetParentOrganisation(context, message.Body.ParentOrganisationId);
            organisationListItem.ParentOrganisationOvoNumber = parentOrganisation.OvoNumber;
            organisationListItem.ParentOrganisationsRelationshipId = message.Body.OrganisationFormalFrameworkId;

            organisationListItem.FormalFrameworkValidities.Add(
                new OrganisationSearchFormalFrameworkValidity
                {
                    ValidFrom = message.Body.ValidFrom,
                    ValidTo = message.Body.ValidTo,
                    OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId,
                });
        }


        // PARENT STUFF
        if (!OrganisationExistsForFormalFramework(context, message.Body.ParentOrganisationId, message.Body.FormalFrameworkId))
        {
            // add parent for formal framework
            var regularParentListItem =
                context.OrganisationSearchList
                    .Include(item => item.OrganisationSearchClassificationValidities)
                    .Single(
                        item =>
                            item.OrganisationId == message.Body.ParentOrganisationId &&
                            item.FormalFrameworkId == null);

            var parentListItemForFormalFramework = new OrganisationSearchListItem
            {
                OrganisationId = regularParentListItem.OrganisationId,
                Name = regularParentListItem.Name,
                ShortName = regularParentListItem.ShortName,
                OvoNumber = regularParentListItem.OvoNumber,
                ValidFrom = regularParentListItem.ValidFrom,
                ValidTo = regularParentListItem.ValidTo,
                FormalFrameworkId = message.Body.FormalFrameworkId,
                ParentOrganisationId = null,
                ParentOrganisation = null,
                ParentOrganisationOvoNumber = null,
                ParentOrganisationsRelationshipId = null,
                OrganisationSearchClassificationValidities = regularParentListItem.OrganisationSearchClassificationValidities.Select(Copy).ToList(),
                FormalFrameworkValidities = new List<OrganisationSearchFormalFrameworkValidity>
                {
                    new()
                    {
                        ValidFrom = message.Body.ValidFrom,
                        ValidTo = message.Body.ValidTo,
                        OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId,
                    },
                },
            };

            await context.OrganisationSearchList.AddAsync(parentListItemForFormalFramework);
        }
        else
        {
            var parentListItemForFormalFramework = context.OrganisationSearchList
                .Include(item => item.FormalFrameworkValidities)
                .Single(
                    item =>
                        item.OrganisationId == message.Body.ParentOrganisationId &&
                        item.FormalFrameworkId == message.Body.FormalFrameworkId);

            parentListItemForFormalFramework.FormalFrameworkValidities.Add(
                new OrganisationSearchFormalFrameworkValidity
                {
                    ValidFrom = message.Body.ValidFrom,
                    ValidTo = message.Body.ValidTo,
                    OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId,
                });
        }

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        // CHILD STUFF
        var organisationListItem =
            context.OrganisationSearchList
                .Include(item => item.FormalFrameworkValidities)
                .Single(
                    item =>
                        item.FormalFrameworkId == message.Body.FormalFrameworkId &&
                        item.OrganisationId == message.Body.OrganisationId);

        organisationListItem.ParentOrganisationId = message.Body.ParentOrganisationId;
        organisationListItem.ParentOrganisation = message.Body.ParentOrganisationName;
        var parentOrganisation = GetParentOrganisation(context, message.Body.ParentOrganisationId);
        organisationListItem.ParentOrganisationOvoNumber = parentOrganisation.OvoNumber;
        organisationListItem.ParentOrganisationsRelationshipId = message.Body.OrganisationFormalFrameworkId;

        var organisationFormalFrameworkValidity =
            organisationListItem.FormalFrameworkValidities.SingleOrDefault(
                validity =>
                    validity.OrganisationFormalFrameworkId == message.Body.OrganisationFormalFrameworkId);

        if (organisationFormalFrameworkValidity != null)
        {
            organisationFormalFrameworkValidity.ValidFrom = message.Body.ValidFrom;
            organisationFormalFrameworkValidity.ValidTo = message.Body.ValidTo;
        }
        else
        {
            organisationListItem.FormalFrameworkValidities.Add(
                new OrganisationSearchFormalFrameworkValidity
                {
                    ValidFrom = message.Body.ValidFrom,
                    ValidTo = message.Body.ValidTo,
                    OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId,
                });
        }

        // PARENT STUFF
        // NEW PARENT STUFF
        if (!OrganisationExistsForFormalFramework(context, message.Body.ParentOrganisationId, message.Body.FormalFrameworkId))
        {
            // add parent for formal framework
            var regularParentListItem =
                context.OrganisationSearchList
                    .Include(item => item.OrganisationSearchClassificationValidities)
                    .Single(
                        item =>
                            item.OrganisationId == message.Body.ParentOrganisationId &&
                            item.FormalFrameworkId == null);

            var parentListItemForFormalFramework = new OrganisationSearchListItem
            {
                OrganisationId = regularParentListItem.OrganisationId,
                Name = regularParentListItem.Name,
                ShortName = regularParentListItem.ShortName,
                OvoNumber = regularParentListItem.OvoNumber,
                ValidFrom = regularParentListItem.ValidFrom,
                ValidTo = regularParentListItem.ValidTo,
                FormalFrameworkId = message.Body.FormalFrameworkId,
                OrganisationSearchClassificationValidities = regularParentListItem.OrganisationSearchClassificationValidities.Select(Copy).ToList(),
                FormalFrameworkValidities = new List<OrganisationSearchFormalFrameworkValidity>
                {
                    new()
                    {
                        ValidFrom = message.Body.ValidFrom,
                        ValidTo = message.Body.ValidTo,
                        OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId,
                    },
                },
                ParentOrganisationId = null,
                ParentOrganisation = null,
                ParentOrganisationOvoNumber = null,
                ParentOrganisationsRelationshipId = null,
            };

            await context.OrganisationSearchList.AddAsync(parentListItemForFormalFramework);
        }
        else
        {
            // Add/Update validity for parent
            var parentListItem =
                context.OrganisationSearchList
                    .Include(item => item.FormalFrameworkValidities)
                    .Single(
                        item =>
                            item.OrganisationId == message.Body.ParentOrganisationId &&
                            item.FormalFrameworkId == message.Body.FormalFrameworkId);

            var parentFormalFrameworkValidity =
                parentListItem.FormalFrameworkValidities.SingleOrDefault(
                    validity =>
                        validity.OrganisationFormalFrameworkId == message.Body.OrganisationFormalFrameworkId);

            if (parentFormalFrameworkValidity != null)
            {
                parentFormalFrameworkValidity.ValidFrom = message.Body.ValidFrom;
                parentFormalFrameworkValidity.ValidTo = message.Body.ValidTo;
            }
            else
            {
                parentListItem.FormalFrameworkValidities.Add(
                    new OrganisationSearchFormalFrameworkValidity
                    {
                        ValidFrom = message.Body.ValidFrom,
                        ValidTo = message.Body.ValidTo,
                        OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId,
                    });
            }
        }

        await context.SaveChangesAsync();

        // OLD PARENT STUFF
        if (!OrganisationHasChildrenForFormalFramework(context, message.Body.PreviousParentOrganisationId, message.Body.FormalFrameworkId) &&
            !OrganisationHasParentForFormalFramework(context, message.Body.PreviousParentOrganisationId, message.Body.FormalFrameworkId))
        {
            RemoveOrganisationFromFormalFramework(context, message.Body.PreviousParentOrganisationId, message.Body.FormalFrameworkId);
        }

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkRemoved> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);

        var organisationListItem =
            context.OrganisationSearchList
                .Include(item => item.FormalFrameworkValidities)
                .Where(item => item.FormalFrameworkValidities.Any(ffv => ffv.OrganisationFormalFrameworkId == message.Body.OrganisationFormalFrameworkId))
                .Single(item => item.OrganisationId == message.Body.OrganisationId);

        var formalFrameworkValidity = organisationListItem.FormalFrameworkValidities.Single(
            validity => validity.OrganisationFormalFrameworkId == message.Body.OrganisationFormalFrameworkId);

        organisationListItem.FormalFrameworkValidities.Remove(formalFrameworkValidity);

        await context.SaveChangesAsync();
    }

    private static bool OrganisationExistsForFormalFramework(OrganisationRegistryContext context, Guid organisationId, Guid formalFrameworkId)
    {
        return context.OrganisationSearchList.Any(
            item =>
                item.FormalFrameworkId == formalFrameworkId &&
                item.OrganisationId == organisationId);
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOrganisationClassificationAdded> message)
    {
        AddOrganisationClassification(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.OrganisationOrganisationClassificationId, message.Body.OrganisationClassificationId, message.Body.OrganisationClassificationTypeId, message.Body.ValidFrom, message.Body.ValidTo);
        await Task.CompletedTask;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboLegalFormOrganisationOrganisationClassificationAdded> message)
    {
        AddOrganisationClassification(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.OrganisationOrganisationClassificationId, message.Body.OrganisationClassificationId, message.Body.OrganisationClassificationTypeId, message.Body.ValidFrom, message.Body.ValidTo);
        await Task.CompletedTask;
    }

    private static void AddOrganisationClassification(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IContextFactory contextFactory,
        Guid organisationId,
        Guid organisationOrganisationClassificationId,
        Guid organisationClassificationId,
        Guid organisationClassificationTypeId,
        DateTime? validFrom,
        DateTime? validTo)
    {
        using var context = contextFactory.CreateTransactional(dbConnection, dbTransaction);
        context.OrganisationSearchList
            .Include(item => item.OrganisationSearchClassificationValidities)
            .Where(item => item.OrganisationId == organisationId)
            .ToList()
            .ForEach(
                item =>
                    item.OrganisationSearchClassificationValidities.Add(
                        new OrganisationSearchClassificationValidity
                        {
                            OrganisationOrganisationClassificationId =
                                organisationOrganisationClassificationId,
                            OrganisationClassificationId = organisationClassificationId,
                            OrganisationClassificationTypeId = organisationClassificationTypeId,
                            ValidFrom = validFrom,
                            ValidTo = validTo,
                        }));

        context.SaveChanges();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboLegalFormOrganisationOrganisationClassificationRemoved> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        context.OrganisationSearchList
            .Include(item => item.OrganisationSearchClassificationValidities)
            .Where(item => item.OrganisationId == message.Body.OrganisationId)
            .ToList()
            .ForEach(
                item =>
                {
                    var organisationClassificationValidities = item.OrganisationSearchClassificationValidities
                        .Where(
                            validity =>
                                validity.OrganisationOrganisationClassificationId ==
                                message.Body.OrganisationOrganisationClassificationId)
                        .ToList();

                    context.OrganisationSearchClassificationValidities.RemoveRange(organisationClassificationValidities);
                });

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        foreach (var organisationListItem in context.OrganisationSearchList.Where(item => item.OrganisationId == message.Body.OrganisationId))
        {
            organisationListItem.Name = message.Body.NameBeforeKboCoupling;
            organisationListItem.ShortName = message.Body.ShortNameBeforeKboCoupling;
        }

        foreach (var child in context.OrganisationSearchList.Where(item => item.ParentOrganisationId == message.Body.OrganisationId))
            child.ParentOrganisation = message.Body.NameBeforeKboCoupling;

        context.OrganisationSearchList
            .Include(item => item.OrganisationSearchClassificationValidities)
            .Where(item => item.OrganisationId == message.Body.OrganisationId)
            .ToList()
            .ForEach(
                item =>
                {
                    var organisationClassificationValidities = item.OrganisationSearchClassificationValidities
                        .Where(
                            validity =>
                                validity.OrganisationOrganisationClassificationId ==
                                message.Body.LegalFormOrganisationOrganisationClassificationIdToCancel)
                        .ToList();

                    context.OrganisationSearchClassificationValidities.RemoveRange(organisationClassificationValidities);
                });

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminationSyncedWithKbo> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        context.OrganisationSearchList
            .Include(item => item.OrganisationSearchClassificationValidities)
            .Where(item => item.OrganisationId == message.Body.OrganisationId)
            .ToList()
            .ForEach(
                item =>
                {
                    var organisationClassificationValidities = item
                        .OrganisationSearchClassificationValidities
                        .Where(
                            validity =>
                                validity.OrganisationOrganisationClassificationId ==
                                message.Body.LegalFormOrganisationOrganisationClassificationIdToTerminate)
                        .ToList();

                    organisationClassificationValidities.ForEach(validity => validity.ValidTo = message.Body.DateOfTermination);
                });

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOrganisationClassificationUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        context.OrganisationSearchList
            .Include(item => item.OrganisationSearchClassificationValidities)
            .Where(item => item.OrganisationId == message.Body.OrganisationId)
            .ToList()
            .ForEach(
                item =>
                {
                    item.OrganisationSearchClassificationValidities
                        .Where(
                            validity =>
                                validity.OrganisationOrganisationClassificationId == message.Body.OrganisationOrganisationClassificationId)
                        .ToList()
                        .ForEach(
                            validity =>
                            {
                                validity.OrganisationOrganisationClassificationId = message.Body.OrganisationOrganisationClassificationId;
                                validity.OrganisationClassificationId = message.Body.OrganisationClassificationId;
                                validity.OrganisationClassificationTypeId = message.Body.OrganisationClassificationTypeId;
                                validity.ValidFrom = message.Body.ValidFrom;
                                validity.ValidTo = message.Body.ValidTo;
                            });
                });

        await context.SaveChangesAsync();
    }

    private static OrganisationSearchClassificationValidity Copy(OrganisationSearchClassificationValidity validity)
        => new()
        {
            OrganisationOrganisationClassificationId = validity.OrganisationOrganisationClassificationId,
            OrganisationClassificationId = validity.OrganisationClassificationId,
            OrganisationClassificationTypeId = validity.OrganisationClassificationTypeId,
            ValidFrom = validity.ValidFrom,
            ValidTo = validity.ValidTo,
        };

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var validities = context.OrganisationSearchClassificationValidities
            .Where(validity => validity.OrganisationClassificationId == message.Body.OrganisationClassificationId);

        foreach (var validity in validities)
            validity.OrganisationClassificationTypeId = message.Body.OrganisationClassificationTypeId;

        await context.SaveChangesAsync();
    }

    private static bool OrganisationHasChildrenForFormalFramework(OrganisationRegistryContext context, Guid parentOrganisationId, Guid formalFrameworkId)
    {
        return context.OrganisationSearchList.Any(
            item =>
                item.FormalFrameworkId == formalFrameworkId &&
                item.ParentOrganisationId == parentOrganisationId);
    }

    private static bool OrganisationHasParentForFormalFramework(OrganisationRegistryContext context, Guid organisationId, Guid formalFrameworkId)
    {
        return context.OrganisationSearchList.Single(
            item =>
                item.FormalFrameworkId == formalFrameworkId &&
                item.OrganisationId == organisationId).ParentOrganisationId.HasValue;
    }

    private static void RemoveOrganisationFromFormalFramework(OrganisationRegistryContext context, Guid organisationId, Guid formalFrameworkId)
    {
        var organisationListItem =
            context.OrganisationSearchList
                .SingleOrDefault(
                    item =>
                        item.FormalFrameworkId == formalFrameworkId &&
                        item.OrganisationId == organisationId);

        if (organisationListItem == null)
            return;

        context.OrganisationSearchList.Remove(organisationListItem);
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
    {
        if (!message.Body.FieldsToTerminate.OrganisationValidity.HasValue)
            return;

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        foreach (var organisationListItem in context.OrganisationSearchList.Where(item => item.OrganisationId == message.Body.OrganisationId))
            organisationListItem.ValidTo = message.Body.FieldsToTerminate.OrganisationValidity;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
    {
        if (!message.Body.FieldsToTerminate.OrganisationValidity.HasValue)
            return;

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        foreach (var organisationListItem in context.OrganisationSearchList.Where(item => item.OrganisationId == message.Body.OrganisationId))
            organisationListItem.ValidTo = message.Body.FieldsToTerminate.OrganisationValidity;

        await context.SaveChangesAsync();
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}
