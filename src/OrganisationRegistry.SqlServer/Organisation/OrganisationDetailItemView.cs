namespace OrganisationRegistry.SqlServer.Organisation
{
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
    using Autofac.Features.OwnedInstances;
    using Day.Events;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Building.Events;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Location.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Purpose.Events;
    using OrganisationRegistry.Security;
    using RebuildProjection = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

    public class OrganisationDetailItem
    {
        public Guid Id { get; set; }

        public string OvoNumber { get; set; }

        public string? KboNumber { get; set; }

        public string Name { get; set; }
        public string? ShortName { get; set; }

        public string? ParentOrganisation { get; set; }
        public Guid? ParentOrganisationId { get; set; }

        /// <summary>
        /// The relationship the ParentOrganisation is in
        /// </summary>
        public Guid? ParentOrganisationOrganisationParentId { get; set; }

        public Guid? FormalFrameworkId { get; set; }
        public Guid? OrganisationClassificationId { get; set; }
        public Guid? OrganisationClassificationTypeId { get; set; }
        public string Description { get; set; }

        public Guid? MainBuildingId { get; set; }
        public string? MainBuildingName { get; set; }

        public Guid? MainLocationId { get; set; }
        public string? MainLocationName { get; set; }

        public string PurposeIds { get; set; }
        public string PurposeNames { get; set; }

        public bool ShowOnVlaamseOverheidSites { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class OrganisationDetailConfiguration : EntityMappingConfiguration<OrganisationDetailItem>
    {
        public override void Map(EntityTypeBuilder<OrganisationDetailItem> b)
        {
            b.ToTable(nameof(OrganisationDetailItemView.ProjectionTables.OrganisationDetail), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.OvoNumber).HasMaxLength(OrganisationListConfiguration.OvoNumberLength).IsRequired();

            b.Property(p => p.Name).HasMaxLength(OrganisationListConfiguration.NameLength).IsRequired();
            b.Property(p => p.ShortName);

            b.Property(p => p.ParentOrganisation);
            b.Property(p => p.ParentOrganisationId);
            b.Property(p => p.ParentOrganisationOrganisationParentId);

            b.Property(p => p.FormalFrameworkId);
            b.Property(p => p.OrganisationClassificationId);
            b.Property(p => p.OrganisationClassificationTypeId);
            b.Property(p => p.Description);

            b.Property(p => p.MainBuildingId);
            b.Property(p => p.MainBuildingName);

            b.Property(p => p.MainLocationId);
            b.Property(p => p.MainLocationName);

            b.Property(p => p.PurposeIds);
            b.Property(p => p.PurposeNames);

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.OvoNumber).IsUnique();
            b.HasIndex(x => x.Name).IsClustered();
            b.HasIndex(x => x.ParentOrganisation);
        }
    }

    public class OrganisationDetailItemView :
        Projection<OrganisationDetailItemView>,
        IEventHandler<OrganisationCreated>,
        IEventHandler<OrganisationCreatedFromKbo>,
        IEventHandler<OrganisationCoupledWithKbo>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationParentUpdated>,
        IEventHandler<ParentAssignedToOrganisation>,
        IEventHandler<ParentClearedFromOrganisation>,
        IEventHandler<MainBuildingAssignedToOrganisation>,
        IEventHandler<MainBuildingClearedFromOrganisation>,
        IEventHandler<BuildingUpdated>,
        IEventHandler<MainLocationAssignedToOrganisation>,
        IEventHandler<MainLocationClearedFromOrganisation>,
        IEventHandler<LocationUpdated>,
        IEventHandler<PurposeUpdated>,
        IReactionHandler<DayHasPassed>
    {
        private readonly IMemoryCaches _memoryCaches;
        private readonly IEventStore _eventStore;
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationDetail
        }

        public OrganisationDetailItemView(
            ILogger<OrganisationDetailItemView> logger,
            IMemoryCaches memoryCaches,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _memoryCaches = memoryCaches;
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreated> message)
        {
            var organisationListItem = new OrganisationDetailItem
            {
                Id = message.Body.OrganisationId,
                Name = message.Body.Name,
                ShortName = message.Body.ShortName,
                OvoNumber = message.Body.OvoNumber,
                Description = message.Body.Description,
                PurposeIds = message.Body.Purposes.ToSeparatedList("|", x => x.Id.ToString()),
                PurposeNames = message.Body.Purposes.OrderBy(x => x.Name).ToSeparatedList("|", x => x.Name),
                ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.OrganisationDetail.AddAsync(organisationListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreatedFromKbo> message)
        {
            var organisationListItem = new OrganisationDetailItem
            {
                Id = message.Body.OrganisationId,
                Name = message.Body.Name,
                ShortName = message.Body.ShortName,
                OvoNumber = message.Body.OvoNumber,
                Description = message.Body.Description,
                PurposeIds = message.Body.Purposes.ToSeparatedList("|", x => x.Id.ToString()),
                PurposeNames = message.Body.Purposes.OrderBy(x => x.Name).ToSeparatedList("|", x => x.Name),
                ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            organisationListItem.KboNumber = message.Body.KboNumber;

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.OrganisationDetail.AddAsync(organisationListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCoupledWithKbo> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationListItem = context.OrganisationDetail.Single(item => item.Id == message.Body.OrganisationId);

                organisationListItem.KboNumber = message.Body.KboNumber;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationListItem = context.OrganisationDetail.Single(item => item.Id == message.Body.OrganisationId);

                organisationListItem.Name = message.Body.Name;
                organisationListItem.ShortName = message.Body.ShortName;
                organisationListItem.Description = message.Body.Description;
                organisationListItem.ValidFrom = message.Body.ValidFrom;
                organisationListItem.ValidTo = message.Body.ValidTo;
                organisationListItem.PurposeIds = message.Body.Purposes.ToSeparatedList("|", x => x.Id.ToString());
                organisationListItem.PurposeNames = message.Body.Purposes.OrderBy(x => x.Name).ToSeparatedList("|", x => x.Name);
                organisationListItem.ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites;

                foreach (var child in context.OrganisationDetail.Where(item => item.ParentOrganisationId == message.Body.OrganisationId))
                    child.ParentOrganisation = message.Body.Name;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationListItem = context.OrganisationDetail.Single(item => item.Id == message.Body.OrganisationId);

                organisationListItem.Name = message.Body.Name;
                organisationListItem.ShortName = message.Body.ShortName;

                foreach (var child in context.OrganisationDetail.Where(item => item.ParentOrganisationId == message.Body.OrganisationId))
                    child.ParentOrganisation = message.Body.Name;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PurposeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationListItems = context.OrganisationDetail.Where(item => item.PurposeIds.Contains(message.Body.PurposeId.ToString()));

                var previousPurposeName = message.Body.PreviousName;
                foreach (var organisationListItem in organisationListItems)
                {
                    var currentNames = string.IsNullOrWhiteSpace(organisationListItem.PurposeNames)
                        ? new List<string>()
                        : organisationListItem.PurposeNames.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries).ToList();

                    currentNames.Remove(previousPurposeName);
                    currentNames.Add(message.Body.Name);

                    organisationListItem.PurposeNames = currentNames.OrderBy(x => x).ToSeparatedList("|", x => x);
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
                var organisationListItem = context.OrganisationDetail.Single(item => item.Id == message.Body.OrganisationId);

                if (organisationListItem.ParentOrganisationOrganisationParentId != message.Body.OrganisationOrganisationParentId)
                    return;

                organisationListItem.ParentOrganisationId = message.Body.ParentOrganisationId;
                organisationListItem.ParentOrganisation = context.OrganisationDetail.Single(item => item.Id == message.Body.ParentOrganisationId).Name;
                organisationListItem.ParentOrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ParentAssignedToOrganisation> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationListItem = context.OrganisationDetail.Single(item => item.Id == message.Body.OrganisationId);

                organisationListItem.ParentOrganisationId = message.Body.ParentOrganisationId;
                organisationListItem.ParentOrganisation = context.OrganisationDetail.Single(item => item.Id == message.Body.ParentOrganisationId).Name;
                organisationListItem.ParentOrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ParentClearedFromOrganisation> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationListItem = context.OrganisationDetail.Single(item => item.Id == message.Body.OrganisationId);

                organisationListItem.ParentOrganisationId = null;
                organisationListItem.ParentOrganisation = null;
                organisationListItem.ParentOrganisationOrganisationParentId = null;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BuildingUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisations = context.OrganisationDetail.Where(x => x.MainBuildingId == message.Body.BuildingId);
                if (!organisations.Any())
                    return;

                foreach (var organisation in organisations)
                    organisation.MainBuildingName = message.Body.Name;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<MainBuildingAssignedToOrganisation> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisation = context.OrganisationDetail.Single(o => o.Id == message.Body.OrganisationId);
                organisation.MainBuildingId = message.Body.MainBuildingId;
                organisation.MainBuildingName = _memoryCaches.BuildingNames[message.Body.MainBuildingId];

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<MainBuildingClearedFromOrganisation> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisation = context.OrganisationDetail.Single(o => o.Id == message.Body.OrganisationId);
                organisation.MainBuildingId = null;
                organisation.MainBuildingName = null;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LocationUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisations = context.OrganisationDetail.Where(x => x.MainLocationId == message.Body.LocationId);
                if (!organisations.Any())
                    return;

                foreach (var organisation in organisations)
                    organisation.MainLocationName = message.Body.FormattedAddress;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<MainLocationAssignedToOrganisation> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisation = context.OrganisationDetail.Single(o => o.Id == message.Body.OrganisationId);
                organisation.MainLocationId = message.Body.MainLocationId;
                organisation.MainLocationName = _memoryCaches.LocationNames[message.Body.MainLocationId];

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<MainLocationClearedFromOrganisation> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisation = context.OrganisationDetail.Single(o => o.Id == message.Body.OrganisationId);
                organisation.MainLocationId = null;
                organisation.MainLocationName = null;

                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }

        public async Task<List<ICommand>> Handle(IEnvelope<DayHasPassed> message)
        {
            using (var context = ContextFactory.Create())
            {
                var organisationDetails = context.OrganisationDetail.ToList();
                return organisationDetails
                    .Select(item => new UpdateRelationshipValidities(new OrganisationId(item.Id), message.Body.NextDate))
                    .Cast<ICommand>()
                    .ToList();
            }
        }
    }
}
