namespace OrganisationRegistry.SqlServer.Organisation
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.OrganisationClassification.Events;

    using RebuildProjection = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

    public class OrganisationListItem
    {
        public int Id { get; set; }

        public Guid OrganisationId { get; set; }

        public string OvoNumber { get; set; }

        public string Name { get; set; }
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

        public List<OrganisationFormalFrameworkValidity> FormalFrameworkValidities { get; set; } = new List<OrganisationFormalFrameworkValidity>();
        public List<OrganisationClassificationValidity> OrganisationClassificationValidities { get; set; } = new List<OrganisationClassificationValidity>();
    }

    public class OrganisationFormalFrameworkValidity
    {
        public int Id { get; set; }
        public Guid OrganisationFormalFrameworkId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class OrganisationClassificationValidity
    {
        public int Id { get; set; }
        public Guid OrganisationClassificationTypeId { get; set; }
        public Guid OrganisationClassificationId { get; set; }
        public Guid OrganisationOrganisationClassificationId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class OrganisationListConfiguration : EntityMappingConfiguration<OrganisationListItem>
    {
        public const int OvoNumberLength = 10;
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<OrganisationListItem> b)
        {
            b.ToTable(nameof(OrganisationListItemView.ProjectionTables.OrganisationList), "OrganisationRegistry")
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
            b.HasMany(p => p.OrganisationClassificationValidities).WithOne().OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.OvoNumber);
            b.HasIndex(x => x.Name).IsClustered();
            b.HasIndex(x => x.ShortName);
            b.HasIndex(x => x.ParentOrganisation);
            b.HasIndex(x => x.FormalFrameworkId);
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class OrganisationFormalFrameworkValidityListConfiguration : EntityMappingConfiguration<OrganisationFormalFrameworkValidity>
    {
        public override void Map(EntityTypeBuilder<OrganisationFormalFrameworkValidity> b)
        {
            b.ToTable(nameof(OrganisationListItemView.ProjectionTables.OrganisationFormalFrameworkValidity), "OrganisationRegistry")
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

    public class OrganisationClassificationValidityListConfiguration : EntityMappingConfiguration<OrganisationClassificationValidity>
    {
        public override void Map(EntityTypeBuilder<OrganisationClassificationValidity> b)
        {
            b.ToTable(nameof(OrganisationListItemView.ProjectionTables.OrganisationClassificationValidity), "OrganisationRegistry")
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

    public class OrganisationListItemView :
        Projection<OrganisationListItemView>,
        IEventHandler<OrganisationCreated>,
        IEventHandler<OrganisationCreatedFromKbo>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationParentUpdated>,
        IEventHandler<ParentAssignedToOrganisation>,
        IEventHandler<ParentClearedFromOrganisation>,
        IEventHandler<FormalFrameworkAssignedToOrganisation>,
        IEventHandler<FormalFrameworkClearedFromOrganisation>,
        IEventHandler<OrganisationFormalFrameworkAdded>,
        IEventHandler<OrganisationFormalFrameworkUpdated>,
        IEventHandler<OrganisationOrganisationClassificationAdded>,
        IEventHandler<KboLegalFormOrganisationOrganisationClassificationAdded>,
        IEventHandler<KboLegalFormOrganisationOrganisationClassificationRemoved>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
        IEventHandler<OrganisationTerminationSyncedWithKbo>,
        IEventHandler<OrganisationOrganisationClassificationUpdated>,
        IEventHandler<OrganisationClassificationUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationFormalFrameworkValidity,
            OrganisationClassificationValidity,
            OrganisationList,
        }

        private readonly IEventStore _eventStore;
        public OrganisationListItemView(
            ILogger<OrganisationListItemView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        private static OrganisationListItem GetParentOrganisation(OrganisationRegistryContext context, Guid parentOrganisationId)
        {
            return context.OrganisationList.Single(item => item.OrganisationId == parentOrganisationId && item.FormalFrameworkId == null);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreated> message)
        {
            var organisationListItem = new OrganisationListItem
            {
                OrganisationId = message.Body.OrganisationId,
                Name = message.Body.Name,
                ShortName = message.Body.ShortName,
                OvoNumber = message.Body.OvoNumber,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.OrganisationList.AddAsync(organisationListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreatedFromKbo> message)
        {
            var organisationListItem = new OrganisationListItem
            {
                OrganisationId = message.Body.OrganisationId,
                Name = message.Body.Name,
                ShortName = message.Body.ShortName,
                OvoNumber = message.Body.OvoNumber,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.OrganisationList.AddAsync(organisationListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                foreach (var organisationListItem in context.OrganisationList.Where(item => item.OrganisationId == message.Body.OrganisationId))
                {
                    organisationListItem.Name = message.Body.Name;
                    organisationListItem.ShortName = message.Body.ShortName;
                    organisationListItem.ValidFrom = message.Body.ValidFrom;
                    organisationListItem.ValidTo = message.Body.ValidTo;
                }

                foreach (var child in context.OrganisationList.Where(item => item.ParentOrganisationId == message.Body.OrganisationId))
                {
                    child.ParentOrganisation = message.Body.Name;
                    child.ParentOrganisationOvoNumber = message.Body.OvoNumber;
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                foreach (var organisationListItem in context.OrganisationList.Where(item => item.OrganisationId == message.Body.OrganisationId))
                {
                    organisationListItem.Name = message.Body.Name;
                    organisationListItem.ShortName = message.Body.ShortName;
                }

                foreach (var child in context.OrganisationList.Where(item => item.ParentOrganisationId == message.Body.OrganisationId))
                {
                    child.ParentOrganisation = message.Body.Name;
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentUpdated> message)
        {
            if (!message.Body.PreviousParentOrganisationId.HasValue)
                return;

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationListItem = context.OrganisationList.SingleOrDefault(item =>
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
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ParentAssignedToOrganisation> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationListItem = context.OrganisationList.Single(item =>
                    item.OrganisationId == message.Body.OrganisationId &&
                    item.FormalFrameworkId == null);

                organisationListItem.ParentOrganisationId = message.Body.ParentOrganisationId;
                var parentOrganisation = GetParentOrganisation(context, message.Body.ParentOrganisationId);
                organisationListItem.ParentOrganisation = parentOrganisation.Name;
                organisationListItem.ParentOrganisationOvoNumber = parentOrganisation.OvoNumber;
                organisationListItem.ParentOrganisationsRelationshipId = message.Body.OrganisationOrganisationParentId;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ParentClearedFromOrganisation> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationListItem = context.OrganisationList.Single(item =>
                    item.OrganisationId == message.Body.OrganisationId &&
                    item.FormalFrameworkId == null);

                organisationListItem.ParentOrganisationId = null;
                organisationListItem.ParentOrganisation = null;
                organisationListItem.ParentOrganisationOvoNumber = null;
                organisationListItem.ParentOrganisationsRelationshipId = null;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkAssignedToOrganisation> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationListItem = context.OrganisationList.Single(item =>
                    item.FormalFrameworkId == message.Body.FormalFrameworkId &&
                    item.OrganisationId == message.Body.OrganisationId);

                organisationListItem.ParentOrganisationId = message.Body.ParentOrganisationId;
                var parentOrganisation = GetParentOrganisation(context, message.Body.ParentOrganisationId);;
                organisationListItem.ParentOrganisation = parentOrganisation.Name;
                organisationListItem.ParentOrganisationOvoNumber = parentOrganisation.OvoNumber;
                organisationListItem.ParentOrganisationsRelationshipId = message.Body.OrganisationFormalFrameworkId;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkClearedFromOrganisation> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationListItem = context.OrganisationList.Single(item =>
                    item.FormalFrameworkId == message.Body.FormalFrameworkId &&
                    item.OrganisationId == message.Body.OrganisationId);

                organisationListItem.ParentOrganisationId = null;
                organisationListItem.ParentOrganisation = null;
                organisationListItem.ParentOrganisationOvoNumber = null;
                organisationListItem.ParentOrganisationsRelationshipId = null;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkAdded> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                // CHILD STUFF
                if (!OrganisationExistsForFormalFramework(context, message.Body.OrganisationId, message.Body.FormalFrameworkId))
                {
                    // copy regular organisation row to create child row
                    var regularOrganisationListItem =
                        context.OrganisationList
                            .Include(item => item.OrganisationClassificationValidities)
                            .Single(item =>
                                item.OrganisationId == message.Body.OrganisationId &&
                                item.FormalFrameworkId == null);

                    var organisationListItemForFormalFramework = new OrganisationListItem
                    {
                        OrganisationId = regularOrganisationListItem.OrganisationId,
                        Name = regularOrganisationListItem.Name,
                        ShortName = regularOrganisationListItem.ShortName,
                        OvoNumber = regularOrganisationListItem.OvoNumber,
                        ValidFrom = regularOrganisationListItem.ValidFrom,
                        ValidTo = regularOrganisationListItem.ValidTo,
                        OrganisationClassificationValidities = regularOrganisationListItem.OrganisationClassificationValidities.Select(Copy).ToList(),
                        FormalFrameworkId = message.Body.FormalFrameworkId,
                        FormalFrameworkValidities = new List<OrganisationFormalFrameworkValidity>
                        {
                            new OrganisationFormalFrameworkValidity
                            {
                                ValidFrom = message.Body.ValidFrom,
                                ValidTo = message.Body.ValidTo,
                                OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId
                            }
                        }
                    };

                    await context.OrganisationList.AddAsync(organisationListItemForFormalFramework);
                }
                else
                {
                    // update child row
                    var organisationListItem =
                        context.OrganisationList
                            .Include(item => item.FormalFrameworkValidities)
                            .Single(item =>
                                item.FormalFrameworkId == message.Body.FormalFrameworkId &&
                                item.OrganisationId == message.Body.OrganisationId);

                    organisationListItem.ParentOrganisationId = message.Body.ParentOrganisationId;
                    organisationListItem.ParentOrganisation = message.Body.ParentOrganisationName;
                    var parentOrganisation = GetParentOrganisation(context, message.Body.ParentOrganisationId);
                    organisationListItem.ParentOrganisationOvoNumber = parentOrganisation.OvoNumber;
                    organisationListItem.ParentOrganisationsRelationshipId = message.Body.OrganisationFormalFrameworkId;

                    organisationListItem.FormalFrameworkValidities.Add(
                        new OrganisationFormalFrameworkValidity
                        {
                            ValidFrom = message.Body.ValidFrom,
                            ValidTo = message.Body.ValidTo,
                            OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId
                        });
                }


                // PARENT STUFF
                if (!OrganisationExistsForFormalFramework(context, message.Body.ParentOrganisationId, message.Body.FormalFrameworkId))
                {
                    // add parent for formal framework
                     var regularParentListItem =
                        context.OrganisationList
                            .Include(item => item.OrganisationClassificationValidities)
                            .Single(item =>
                                item.OrganisationId == message.Body.ParentOrganisationId &&
                                item.FormalFrameworkId == null);

                    var parentListItemForFormalFramework = new OrganisationListItem
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
                        OrganisationClassificationValidities = regularParentListItem.OrganisationClassificationValidities.Select(Copy).ToList(),
                        FormalFrameworkValidities = new List<OrganisationFormalFrameworkValidity>
                        {
                            new OrganisationFormalFrameworkValidity
                            {
                                ValidFrom = message.Body.ValidFrom,
                                ValidTo = message.Body.ValidTo,
                                OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId
                            }
                        }
                    };

                    await context.OrganisationList.AddAsync(parentListItemForFormalFramework);
                }
                else
                {
                    var parentListItemForFormalFramework = context.OrganisationList
                        .Include(item => item.FormalFrameworkValidities)
                        .Single(item =>
                            item.OrganisationId == message.Body.ParentOrganisationId &&
                            item.FormalFrameworkId == message.Body.FormalFrameworkId);

                    parentListItemForFormalFramework.FormalFrameworkValidities.Add(
                        new OrganisationFormalFrameworkValidity
                        {
                            ValidFrom = message.Body.ValidFrom,
                            ValidTo = message.Body.ValidTo,
                            OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId
                        });
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                // CHILD STUFF
                var organisationListItem =
                    context.OrganisationList
                        .Include(item => item.FormalFrameworkValidities)
                        .Single(item =>
                            item.FormalFrameworkId == message.Body.FormalFrameworkId &&
                            item.OrganisationId == message.Body.OrganisationId);

                organisationListItem.ParentOrganisationId = message.Body.ParentOrganisationId;
                organisationListItem.ParentOrganisation = message.Body.ParentOrganisationName;
                var parentOrganisation = GetParentOrganisation(context, message.Body.ParentOrganisationId);
                organisationListItem.ParentOrganisationOvoNumber = parentOrganisation.OvoNumber;
                organisationListItem.ParentOrganisationsRelationshipId = message.Body.OrganisationFormalFrameworkId;

                var organisationFormalFrameworkValidity =
                    organisationListItem.FormalFrameworkValidities.SingleOrDefault(validity =>
                        validity.OrganisationFormalFrameworkId == message.Body.OrganisationFormalFrameworkId);

                if (organisationFormalFrameworkValidity != null)
                {
                    organisationFormalFrameworkValidity.ValidFrom = message.Body.ValidFrom;
                    organisationFormalFrameworkValidity.ValidTo = message.Body.ValidTo;
                }
                else
                {
                    organisationListItem.FormalFrameworkValidities.Add(
                        new OrganisationFormalFrameworkValidity
                        {
                            ValidFrom = message.Body.ValidFrom,
                            ValidTo = message.Body.ValidTo,
                            OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId
                        });
                }

                // PARENT STUFF
                // NEW PARENT STUFF
                if (!OrganisationExistsForFormalFramework(context, message.Body.ParentOrganisationId, message.Body.FormalFrameworkId))
                {
                    // add parent for formal framework
                    var regularParentListItem =
                        context.OrganisationList
                            .Include(item => item.OrganisationClassificationValidities)
                            .Single(item =>
                                item.OrganisationId == message.Body.ParentOrganisationId &&
                                item.FormalFrameworkId == null);

                    var parentListItemForFormalFramework = new OrganisationListItem
                    {
                        OrganisationId = regularParentListItem.OrganisationId,
                        Name = regularParentListItem.Name,
                        ShortName = regularParentListItem.ShortName,
                        OvoNumber = regularParentListItem.OvoNumber,
                        ValidFrom = regularParentListItem.ValidFrom,
                        ValidTo = regularParentListItem.ValidTo,
                        FormalFrameworkId = message.Body.FormalFrameworkId,
                        OrganisationClassificationValidities = regularParentListItem.OrganisationClassificationValidities.Select(Copy).ToList(),
                        FormalFrameworkValidities = new List<OrganisationFormalFrameworkValidity>
                        {
                            new OrganisationFormalFrameworkValidity
                            {
                                ValidFrom = message.Body.ValidFrom,
                                ValidTo = message.Body.ValidTo,
                                OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId
                            }
                        },
                        ParentOrganisationId = null,
                        ParentOrganisation = null,
                        ParentOrganisationOvoNumber = null,
                        ParentOrganisationsRelationshipId = null
                    };

                    await context.OrganisationList.AddAsync(parentListItemForFormalFramework);
                }
                else
                {
                    // Add/Update validity for parent
                    var parentListItem =
                        context.OrganisationList
                            .Include(item => item.FormalFrameworkValidities)
                            .Single(item =>
                                item.OrganisationId == message.Body.ParentOrganisationId &&
                                item.FormalFrameworkId == message.Body.FormalFrameworkId);

                    var parentFormalFrameworkValidity =
                        parentListItem.FormalFrameworkValidities.SingleOrDefault(validity =>
                            validity.OrganisationFormalFrameworkId == message.Body.OrganisationFormalFrameworkId);

                    if (parentFormalFrameworkValidity != null)
                    {
                        parentFormalFrameworkValidity.ValidFrom = message.Body.ValidFrom;
                        parentFormalFrameworkValidity.ValidTo = message.Body.ValidTo;
                    }
                    else
                    {
                        parentListItem.FormalFrameworkValidities.Add(
                            new OrganisationFormalFrameworkValidity
                            {
                                ValidFrom = message.Body.ValidFrom,
                                ValidTo = message.Body.ValidTo,
                                OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId
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
        }

        private static bool OrganisationExistsForFormalFramework(OrganisationRegistryContext context, Guid organisationId, Guid formalFrameworkId)
        {
            return context.OrganisationList.Any(item =>
                item.FormalFrameworkId == formalFrameworkId &&
                item.OrganisationId == organisationId);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOrganisationClassificationAdded> message)
        {
            AddOrganisationClassification(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.OrganisationOrganisationClassificationId, message.Body.OrganisationClassificationId, message.Body.OrganisationClassificationTypeId, message.Body.ValidFrom, message.Body.ValidTo);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboLegalFormOrganisationOrganisationClassificationAdded> message)
        {
            AddOrganisationClassification(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.OrganisationOrganisationClassificationId, message.Body.OrganisationClassificationId, message.Body.OrganisationClassificationTypeId, message.Body.ValidFrom, message.Body.ValidTo);
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
            using (var context = contextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                context.OrganisationList
                    .Include(item => item.OrganisationClassificationValidities)
                    .Where(item => item.OrganisationId == organisationId)
                    .ToList()
                    .ForEach(item =>
                        item.OrganisationClassificationValidities.Add(
                            new OrganisationClassificationValidity
                            {
                                OrganisationOrganisationClassificationId =
                                    organisationOrganisationClassificationId,
                                OrganisationClassificationId = organisationClassificationId,
                                OrganisationClassificationTypeId = organisationClassificationTypeId,
                                ValidFrom = validFrom,
                                ValidTo = validTo
                            }));

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboLegalFormOrganisationOrganisationClassificationRemoved> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                context.OrganisationList
                    .Include(item => item.OrganisationClassificationValidities)
                    .Where(item => item.OrganisationId == message.Body.OrganisationId)
                    .ToList()
                    .ForEach(item =>
                    {
                        var organisationClassificationValidities = item.OrganisationClassificationValidities
                            .Where(validity =>
                                validity.OrganisationOrganisationClassificationId ==
                                message.Body.OrganisationOrganisationClassificationId)
                            .ToList();

                        context.OrganisationClassificationValidities.RemoveRange(organisationClassificationValidities);
                    });

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                foreach (var organisationListItem in context.OrganisationList.Where(item => item.OrganisationId == message.Body.OrganisationId))
                {
                    organisationListItem.Name = message.Body.NameBeforeKboCoupling;
                    organisationListItem.ShortName = message.Body.ShortNameBeforeKboCoupling;
                }

                foreach (var child in context.OrganisationList.Where(item => item.ParentOrganisationId == message.Body.OrganisationId))
                {
                    child.ParentOrganisation = message.Body.NameBeforeKboCoupling;
                }

                context.OrganisationList
                    .Include(item => item.OrganisationClassificationValidities)
                    .Where(item => item.OrganisationId == message.Body.OrganisationId)
                    .ToList()
                    .ForEach(item =>
                    {
                        var organisationClassificationValidities = item.OrganisationClassificationValidities
                            .Where(validity =>
                                validity.OrganisationOrganisationClassificationId ==
                                message.Body.LegalFormOrganisationOrganisationClassificationIdToCancel)
                            .ToList();

                        context.OrganisationClassificationValidities.RemoveRange(organisationClassificationValidities);
                    });

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminationSyncedWithKbo> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                context.OrganisationList
                    .Include(item => item.OrganisationClassificationValidities)
                    .Where(item => item.OrganisationId == message.Body.OrganisationId)
                    .ToList()
                    .ForEach(item =>
                    {
                        var organisationClassificationValidities = item.OrganisationClassificationValidities
                            .Where(validity =>
                                validity.OrganisationOrganisationClassificationId ==
                                message.Body.LegalFormOrganisationOrganisationClassificationIdToTerminate)
                            .ToList();

                        organisationClassificationValidities.ForEach(validity => validity.ValidTo = message.Body.DateOfTermination);
                    });

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOrganisationClassificationUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                context.OrganisationList
                    .Include(item => item.OrganisationClassificationValidities)
                    .Where(item => item.OrganisationId == message.Body.OrganisationId)
                    .ToList()
                    .ForEach(item =>
                    {
                        item.OrganisationClassificationValidities
                            .Where(validity =>
                                validity.OrganisationOrganisationClassificationId == message.Body.OrganisationOrganisationClassificationId)
                            .ToList()
                            .ForEach(validity =>
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
        }

        private static OrganisationClassificationValidity Copy(OrganisationClassificationValidity validity)
        {
            return new OrganisationClassificationValidity
            {
                OrganisationOrganisationClassificationId = validity.OrganisationOrganisationClassificationId,
                OrganisationClassificationId = validity.OrganisationClassificationId,
                OrganisationClassificationTypeId = validity.OrganisationClassificationTypeId,
                ValidFrom = validity.ValidFrom,
                ValidTo = validity.ValidTo
            };
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var validities = context.OrganisationClassificationValidities
                    .Where(validity => validity.OrganisationClassificationId == message.Body.OrganisationClassificationId);

                foreach (var validity in validities)
                {
                    validity.OrganisationClassificationTypeId = message.Body.OrganisationClassificationTypeId;
                }
                await context.SaveChangesAsync();
            }
        }

        private static bool OrganisationHasChildrenForFormalFramework(OrganisationRegistryContext context, Guid parentOrganisationId, Guid formalFrameworkId)
        {
            return context.OrganisationList.Any(item =>
                item.FormalFrameworkId == formalFrameworkId &&
                item.ParentOrganisationId == parentOrganisationId);
        }

        private static bool OrganisationHasParentForFormalFramework(OrganisationRegistryContext context, Guid organisationId, Guid formalFrameworkId)
        {
            return context.OrganisationList.Single(item =>
                item.FormalFrameworkId == formalFrameworkId &&
                item.OrganisationId == organisationId).ParentOrganisationId.HasValue;
        }

        private static void RemoveOrganisationFromFormalFramework(OrganisationRegistryContext context, Guid organisationId, Guid formalFrameworkId)
        {
            var organisationListItem =
                context.OrganisationList
                    .Single(item =>
                        item.FormalFrameworkId == formalFrameworkId &&
                        item.OrganisationId == organisationId);

            if (organisationListItem == null)
                return;

            context.OrganisationList.Remove(organisationListItem);
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
